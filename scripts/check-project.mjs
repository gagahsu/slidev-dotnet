// 檢查 EShop 課程專案（eshop/ch01 … eshop/ch10）
//
// 1. 每一步的參考解答都要：dotnet build 沒有錯誤與警告、dotnet test 全部通過
// 2. 投影片中第一行是 eshop/<路徑> 的程式碼區塊，內容必須和 eshop/chNN/<路徑> 檔案一致：
//      C#、JS、jsonc：// eshop/EShop.Web/Program.cs
//      Razor：        @* eshop/EShop.Web/Views/Products/Index.cshtml *@
//      XML：          <!-- eshop/EShop.Web/EShop.Web.csproj -->
//    - 區塊內容（去掉第一行）必須依序出現在檔案中
//    - 單獨一行的 `// ...`（Razor 用 `@* ... *@`、XML 用 `<!-- ... -->`）代表省略，前後兩段分開比對
//
// 用法：pnpm check:project            （全部）
//       pnpm check:project ch05       （只檢查 ch05）
//       pnpm check:project --no-test  （只比對投影片與檔案，不 build、不跑測試）
import { existsSync, readdirSync, readFileSync } from 'fs'
import { spawnSync } from 'child_process'
import { join } from 'path'
import { fileURLToPath } from 'url'

const root = fileURLToPath(new URL('..', import.meta.url))
const args = process.argv.slice(2)
const filter = args.find(a => !a.startsWith('--'))
const runTests = !args.includes('--no-test')

const decks = readdirSync(root).filter(f => /^ch\d+.*\.md$/.test(f)).sort()
const header = /^\s*(?:\/\/|@\*|<!--)\s*eshop\/(\S+?)(?:\s*(?:\*@|-->))?\s*$/
const elision = /^\s*(?:\/\/|@\*|<!--)\s*\.\.\.\s*(?:\*@|-->)?\s*$/
let failed = 0
let excerpts = 0

const fail = msg => {
  failed++
  console.error('✘ ' + msg)
}
const read = file => readFileSync(file, 'utf8').replace(/^﻿/, '').replace(/\r\n/g, '\n')

for (const deck of decks) {
  const ch = deck.slice(0, 4)
  if (filter && ch !== filter) continue
  const dir = join(root, 'eshop', ch)
  const lines = read(join(root, deck)).split('\n')

  // 1. 投影片摘錄與檔案內容比對
  for (let i = 0; i < lines.length; i++) {
    if (!/^```\w*/.test(lines[i])) continue
    let end = i + 1
    while (end < lines.length && !/^```\s*$/.test(lines[end])) end++
    const body = lines.slice(i + 1, end)
    const where = `${deck}:${i + 2}`
    i = end
    const m = body[0]?.match(header)
    if (!m) continue
    excerpts++
    const file = join(dir, m[1])
    if (!existsSync(file)) {
      fail(`${where} 找不到檔案 eshop/${ch}/${m[1]}`)
      continue
    }
    const content = read(file)
    const chunks = [[]]
    for (const l of body.slice(1)) {
      if (elision.test(l)) chunks.push([])
      else chunks.at(-1).push(l)
    }
    let pos = 0
    for (const chunk of chunks) {
      while (chunk.length && chunk[0].trim() === '') chunk.shift()
      while (chunk.length && chunk.at(-1).trim() === '') chunk.pop()
      if (!chunk.length) continue
      const text = chunk.join('\n')
      const at = content.indexOf(text, pos)
      if (at < 0) {
        // 找出第一行對不上的地方，方便修正
        const bad = chunk.find(l => !content.includes(l)) ?? chunk[0]
        fail(`${where} 與 eshop/${ch}/${m[1]} 不一致：\n    ${bad}`)
        break
      }
      pos = at + text.length
    }
  }

  // 2. 參考解答本身要能 build、測試要通過
  if (!runTests || !existsSync(dir)) continue
  const dotnet = (...a) => spawnSync('dotnet', a, {
    cwd: dir,
    encoding: 'utf8',
    env: { ...process.env, DOTNET_CLI_TELEMETRY_OPTOUT: '1', DOTNET_NOLOGO: '1' },
  })
  const build = dotnet('build', '--no-incremental')
  const warnings = Number(build.stdout.match(/(\d+) Warning\(s\)/)?.[1] ?? 0)
  if (build.status !== 0) {
    fail(`eshop/${ch} dotnet build 失敗\n${build.stdout.split('\n').filter(l => / error /.test(l)).join('\n')}`)
    continue
  }
  if (warnings > 0) {
    fail(`eshop/${ch} dotnet build 有 ${warnings} 個警告\n${build.stdout.split('\n').filter(l => / warning /.test(l)).join('\n')}`)
  }
  const hasTests = existsSync(join(dir, 'EShop.Tests'))
  if (hasTests) {
    const test = dotnet('test', '--no-build')
    if (test.status !== 0) {
      fail(`eshop/${ch} dotnet test 失敗\n${(test.stdout + test.stderr).trim()}`)
      continue
    }
  }
  const summary = hasTests ? '，測試通過' : '（沒有測試專案）'
  console.log(`✔ eshop/${ch}：build 通過${summary}`)
}

console.log(`比對 ${excerpts} 個投影片摘錄`)
if (failed) {
  console.error(`共 ${failed} 個問題`)
  process.exit(1)
}
console.log('✔ 全部通過')
