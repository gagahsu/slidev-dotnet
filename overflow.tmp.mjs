import { chromium } from 'playwright-chromium'
const b = await chromium.launch({ executablePath: '/opt/pw-browsers/chromium' })
const p = await b.newPage({ viewport: { width: 1280, height: 720 } })
await p.goto('http://localhost:3032/1', { waitUntil: 'networkidle' })
await p.waitForTimeout(3000)
const total = await p.evaluate(() => window.__slidev__?.nav?.total ?? 0)
console.log('total', total)
for (let n = 1; n <= total; n++) {
  await p.goto(`http://localhost:3032/${n}`, { waitUntil: 'domcontentloaded' })
  await p.waitForTimeout(400)
  const r = await p.evaluate(() => {
    const els = [...document.querySelectorAll('.slidev-layout')].filter(e => e.offsetParent !== null)
    const el = els[els.length - 1]
    if (!el) return null
    return { sh: el.scrollHeight, ch: el.clientHeight, h1: el.querySelector('h1')?.textContent?.trim() ?? '' }
  })
  if (r && r.sh > r.ch + 4) console.log(`${n}\t${r.sh - r.ch}\t${r.h1}`)
}
await b.close()
