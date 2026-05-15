$(function () {

    var service = portal.ibanez.qrCodes.qrCode;

    var createModal = new abp.ModalManager({
        viewUrl: '/QrCodes/CreateModal'
    });

    var editModal = new abp.ModalManager({
        viewUrl: '/QrCodes/EditModal'
    });

    var dataTable = $('#QrCodesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,

            ajax: abp.libs.datatables.createAjax(service.getList, function () {
                return {
                    machineId: $('#MachineId').val()
                };
            }),

            columnDefs: [
                {
                    rowAction: {
                        items: [
                            {
                                text: 'Ver QR',
                                action: function (data) {
                                    window.open('/QrCodes/Image/' + data.record.id, '_blank');
                                }
                            },
                            {
                            text: 'Documentos',
                            action: function (data) {
                                window.location.href = '/QrCodes/Documents?qrCodeId=' + data.record.id;
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
                                confirmMessage: function () {
                                    return '¿Seguro que deseas eliminar este QR?';
                                },
                                action: function (data) {
                                    service.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.success('QR eliminado correctamente');
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    },
                    width: "80px"
                },
                {
                    title: 'Nombre',
                    data: "name"
                },
                {
                    title: 'Código',
                    data: "code"
                },
                {
                    title: 'Descripción',
                    data: "description"
                },
                {
                    title: 'Activo',
                    data: "isActive",
                    render: function (data) {
                        return data
                            ? '<span class="badge bg-success">Sí</span>'
                            : '<span class="badge bg-danger">No</span>';
                    }
                }
            ]
        })
    );

    $('#NewQrCodeButton').click(function (e) {
        e.preventDefault();
        createModal.open({
            machineId: $('#MachineId').val()
        });
    });

    createModal.onResult(function () {
        location.reload();
    });

    editModal.onResult(function () {
        location.reload();
    });

});
