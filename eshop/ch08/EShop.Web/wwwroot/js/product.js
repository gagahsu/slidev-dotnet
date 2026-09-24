document.addEventListener('DOMContentLoaded', () => {
  window.productTable = new DataTable('#tblData', {
    ajax: '/Admin/Product/GetAll',
    columns: [
      { data: 'name' },
      { data: 'category' },
      { data: 'origin' },
      { data: 'price', render: DataTable.render.number(',', '.', 0, 'NT$ ') },
      { data: 'stock' },
      {
        data: 'id', orderable: false,
        render: id => `
          <a href="/Admin/Product/Upsert/${id}" class="btn btn-sm btn-outline-primary">編輯</a>
          <button onclick="deleteProduct(${id})" class="btn btn-sm btn-outline-danger">刪除</button>`
      }
    ],
    language: { url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/zh-HANT.json' }
  });
});

async function deleteProduct(id) {
  const result = await Swal.fire({ title: '確定要刪除嗎？', text: '刪除後無法復原',
    icon: 'warning', showCancelButton: true, confirmButtonText: '刪除', cancelButtonText: '取消' });
  if (!result.isConfirmed) return;

  const res = await fetch(`/Admin/Product/Delete/${id}`, { method: 'DELETE' });
  const json = await res.json();
  json.success ? toastr.success(json.message) : toastr.error(json.message);
  window.productTable.ajax.reload();
}
