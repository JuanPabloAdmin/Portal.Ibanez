$(function () {

    var l = abp.localization.getResource('Ibanez');

    var service = portal.ibanez.customers.customer;

    var createModal = new abp.ModalManager({
        viewUrl: '/Customers/CreateModal'
    });

    var editModal = new abp.ModalManager({
        viewUrl: '/Customers/EditModal'
    });

    var dataTable = $('#CustomersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,

            ajax: abp.libs.datatables.createAjax(service.getList),

            columnDefs: [
                {
                    rowAction: {
                        items: [
                            {
                                text: 'Máquinas',
                                action: function (data) {
                                    window.location.href = '/Machines?customerId=' + data.record.id;
                                }
                            },
                            {
                                text: 'Editar',
                                action: function (data) {
                                    editModal.open({
                                        id: data.record.id
                                    });
                                }
                            },
                            {
                                text: 'Eliminar',
                                confirmMessage: function (data) {
                                    return '¿Seguro que deseas eliminar el cliente "' + data.record.commercialName + '"?';
                                },
                                action: function (data) {
                                    service.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.success('Cliente eliminado correctamente');
                                            dataTable.ajax.reload(null, false);
                                        });
                                }
                            }
                        ]
                    },
                    width: "80px"
                },
                {
                    title: 'Nombre Comercial',
                    data: "commercialName"
                },
                {
                    title: 'Nombre Fiscal',
                    data: "fiscalName"
                },
                {
                    title: 'CIF',
                    data: "taxId"
                },
                {
                    title: 'Teléfono',
                    data: "phone"
                },
                {
                    title: 'Email',
                    data: "email"
                }
            ]
        })
    );

    $('#NewCustomerButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    createModal.onResult(function () {
        location.reload();
        //dataTable.ajax.reload(null, false);
    });

    editModal.onResult(function () {
        location.reload();
    });

  

});