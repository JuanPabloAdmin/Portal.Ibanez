$(function () {

    var service = portal.ibanez.documentFolders.documentFolder;

    var createModal = new abp.ModalManager({
        viewUrl: '/DocumentFolders/CreateModal'
    });

    var editModal = new abp.ModalManager({
        viewUrl: '/DocumentFolders/EditModal'
    });

    var dataTable = $('#DocumentFoldersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            searching: true,
            scrollX: true,
            order: [[1, 'asc']],

            ajax: abp.libs.datatables.createAjax(
                service.getList,
                function () {
                    return {
                        machineId: $('#MachineId').val()
                    };
                }
            ),

            columnDefs: [
                {
                    rowAction: {
                        items: [
                            {
                                text: 'Documentos',
                                action: function (data) {
                                    window.location.href =
                                        '/MachineDocuments?machineId=' +
                                        $('#MachineId').val() +
                                        '&documentFolderId=' +
                                        data.record.id;
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
                                    return '¿Seguro que deseas eliminar la carpeta "' +
                                        data.record.name +
                                        '"?';
                                },
                                action: function (data) {
                                    service.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.success(
                                                'Carpeta eliminada correctamente'
                                            );
                                            location.reload();
                                        });
                                }
                            }
                        ]
                    },
                    width: '90px'
                },
                {
                    title: 'Nombre',
                    data: 'name'
                },
                {
                    title: 'Descripción',
                    data: 'description'
                },
                {
                    title: 'Activa',
                    data: 'isActive',
                    render: function (data) {
                        return data
                            ? '<span class="badge bg-success">Sí</span>'
                            : '<span class="badge bg-secondary">No</span>';
                    }
                }
            ]
        })
    );

    $('#NewDocumentFolderButton').click(function (e) {
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