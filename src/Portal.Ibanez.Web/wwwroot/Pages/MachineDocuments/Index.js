$(function () {

    var service = portal.ibanez.documents.machineDocument;

    var createModal = new abp.ModalManager({
        viewUrl: '/MachineDocuments/CreateModal'
    });

    var editModal = new abp.ModalManager({
        viewUrl: '/MachineDocuments/EditModal'
    });

    var dataTable = $('#MachineDocumentsTable').DataTable(
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
                                text: 'Descargar',
                                action: function (data) {
                                    window.location.href = '/MachineDocuments/Download/' + data.record.id;
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
                                    return '¿Seguro que deseas eliminar este documento?';
                                },
                                action: function (data) {
                                    service.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.success('Documento eliminado correctamente');
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    },
                    width: "80px"
                },
                {
                    title: 'Título',
                    data: "title"
                },
                {
                    title: 'Archivo',
                    data: "fileName"
                },
                {
                    title: 'Versión',
                    data: "version"
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

    $('#NewMachineDocumentButton').click(function (e) {
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