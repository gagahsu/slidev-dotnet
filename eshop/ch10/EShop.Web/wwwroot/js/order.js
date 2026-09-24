new DataTable('#tblOrder', {
  ajax: `/Admin/Order/GetAll${location.search}`,        // 帶上 ?status=...
  order: [[3, 'desc']],
  columns: [
    { data: 'id' }, { data: 'name' }, { data: 'email' }, { data: 'orderDate' },
    { data: 'total', render: DataTable.render.number(',', '.', 0, 'NT$ ') },
    { data: 'status', render: (d, t, row) => `<span class="badge ${row.badge}">${d}</span>` },
    { data: 'id', render: id => `<a href="/Admin/Order/Details/${id}" class="btn btn-sm btn-outline-primary">詳情</a>` }
  ],
  language: { url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/zh-HANT.json' }
});
