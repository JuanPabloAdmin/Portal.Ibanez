$(function () {

    var service = portal.ibanez.machineTypes.machineType;

    var createModal = new abp.ModalManager({
        viewUrl: '/MachineTypes/CreateModal'
    });

    var editModal = new abp.ModalManager({
        viewUrl: '/MachineTypes/EditModal'
    });

    var dataTable = $('#MachineTypesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,

            ajax: abp.libs.datatables.createAjax(service.getList),

            columnDefs: [
                {
                    rowAction: {
                        items:
                            [
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
                                        return '¿Seguro que deseas eliminar este tipo de máquina?';
                                    },
                                    action: function (data) {
                                        service.delete(data.record.id)
                                            .then(function () {
                                                abp.notify.success('Tipo eliminado correctamente');
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
                }
            ]
        })
    );

    $('#NewMachineTypeButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    createModal.onResult(function () {
        location.reload();
    });

    editModal.onResult(function () {
        location.reload();
    });

});