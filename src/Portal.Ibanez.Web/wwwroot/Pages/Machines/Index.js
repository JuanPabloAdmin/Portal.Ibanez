$(function () {

    var service = portal.ibanez.machines.machine;

    var createModal = new abp.ModalManager({
        viewUrl: '/Machines/CreateModal'
    });

    var editModal = new abp.ModalManager({
        viewUrl: '/Machines/EditModal'
    });

    var dataTable = $('#MachinesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,

            ajax: abp.libs.datatables.createAjax(service.getList, function () {
                return {
                    customerId: $('#CustomerId').val()
                };
            }),

            columnDefs: [
                {
                    rowAction: {
                        items: [
                            {
                                text: 'Documentos',
                                action: function (data) {
                                    window.location.href = '/MachineDocuments?machineId=' + data.record.id;
                                }
                            },
                            {
                                text: 'QR Codes',
                                action: function (data) {
                                    window.location.href = '/QrCodes?machineId=' + data.record.id;
                                }
                            },
                            {
                                text: 'Editar',
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: 'Eliminar',
                                confirmMessage: function () {
                                    return '¿Seguro que deseas eliminar esta máquina?';
                                },
                                action: function (data) {
                                    service.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.success('Máquina eliminada correctamente');
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    },
                    width: "80px"
                },
                {
                    title: 'Cliente',
                    data: "customerCommercialName"
                },
                {
                    title: 'Tipo de máquina',
                    data: "machineTypeName"
                },
                {
                    title: 'Nº pedido',
                    data: "orderNumber"
                },
                {
                    title: 'Nº armario',
                    data: "cabinetNumber"
                },
                {
                    title: 'Fecha fabricación',
                    data: "manufacturingDate",
                    render: function (data) {
                        return data ? luxon.DateTime.fromISO(data).toFormat('dd/MM/yyyy') : '';
                    }
                },
                {
                    title: 'Fecha entrega',
                    data: "deliveryDate",
                    render: function (data) {
                        return data ? luxon.DateTime.fromISO(data).toFormat('dd/MM/yyyy') : '';
                    }
                }
            ]
        })
    );

    $('#NewMachineButton').click(function (e) {
        e.preventDefault();
        createModal.open({
            customerId: $('#CustomerId').val()
        });
    });

    createModal.onResult(function () {
        location.reload();
    });

    editModal.onResult(function () {
        location.reload();
    });

});