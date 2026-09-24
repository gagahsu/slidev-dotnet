document.addEventListener('DOMContentLoaded', () => {
  window.storeTable = new DataTable('#tblStore', {
    ajax: '/Admin/Store/GetAll',
    columns: [
      { data: 'name' },
      { data: 'city' },
      { data: 'streetAddress' },
      { data: 'phoneNumber' },
      {
        data: 'id', orderable: false,
        render: id => `
          <a href="/Admin/Store/Upsert/${id}" class="btn btn-sm btn-outline-primary">編輯</a>
          <button onclick="deleteStore(${id})" class="btn btn-sm btn-outline-danger">刪除</button>`
      }
    ],
    language: { url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/zh-HANT.json' }
  });
});

async function deleteStore(id) {
  const result = await Swal.fire({ title: '確定要刪除嗎？', icon: 'warning',
    showCancelButton: true, confirmButtonText: '刪除', cancelButtonText: '取消' });
  if (!result.isConfirmed) return;

  const res = await fetch(`/Admin/Store/Delete/${id}`, { method: 'DELETE' });
  const json = await res.json();
  json.success ? toastr.success(json.message) : toastr.error(json.message);
  window.storeTable.ajax.reload();
}
