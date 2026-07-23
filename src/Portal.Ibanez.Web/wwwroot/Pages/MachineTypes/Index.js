$(function () {
    var service = portal.ibanez.machineTypes.machineType;

    var createModal = new abp.ModalManager({ viewUrl: abp.appPath + 'MachineTypes/CreateModal' });
    var editModal = new abp.ModalManager({ viewUrl: abp.appPath + 'MachineTypes/EditModal' });

    function escapeHtml(value) {
        return $('<div>').text(value || '').html();
    }
    function escapeHtmlAttribute(value) {
        return String(value || '').replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    // Función principal para cargar y dibujar las tarjetas
    function loadMachineTypes() {
        // Obtenemos una cantidad alta para poder filtrar en cliente
        service.getList({ maxResultCount: 1000 }).then(function (result) {
            var container = $('#MachineTypesGrid');
            container.empty();

            if (!result.items || result.items.length === 0) {
                container.append(`
                    <div class="col-12 text-center py-5 w-100">
                        <div class="bg-white p-5 rounded-4 border text-muted shadow-sm d-flex flex-column align-items-center">
                            <div class="mb-3 p-3 bg-light rounded-circle text-secondary">
                                <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" fill="currentColor" viewBox="0 0 16 16">
                                    <path d="M13 2.5a1.5 1.5 0 0 1 3 0v11a1.5 1.5 0 0 1-3 0v-.5H3v.5a1.5 1.5 0 0 1-3 0v-11a1.5 1.5 0 0 1 3 0v.5h10v-.5zM3 2a.5.5 0 0 0-.5.5v11a.5.5 0 0 0 .5.5h10a.5.5 0 0 0 .5-.5v-11A.5.5 0 0 0 13 2H3z"/>
                                </svg>
                            </div>
                            <h4 class="h5 text-dark fw-bold mb-1">Sin resultados</h4>
                            <p class="mb-0 text-secondary" style="font-size: 0.9rem;">Aún no se ha registrado ningún tipo de máquina.</p>
                        </div>
                    </div>
                `);
                return;
            }

            // Dibujar cada tarjeta
            result.items.forEach(function (type) {
                container.append(`
                    <div class="col machine-type-wrapper">
                        <div class="machine-type-card shadow-sm">
                            <div class="card-body-custom">
                                <div class="icon-wrapper shadow-sm">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="26" height="26" fill="currentColor" viewBox="0 0 16 16">
                                        <path d="M8.186 1.113a.5.5 0 0 0-.372 0L1.846 3.5 8 5.961 14.154 3.5 8.186 1.113zM15 4.239l-6.5 2.6v7.922l6.5-2.6V4.24zM7.5 14.762V6.838L1 4.239v7.923l6.5 2.6zM7.443.184a1.5 1.5 0 0 1 1.114 0l7.129 2.852A.5.5 0 0 1 16 3.5v8.662a1 1 0 0 1-.629.928l-7.185 2.874a.5.5 0 0 1-.372 0L.63 13.09a1 1 0 0 1-.63-.928V3.5a.5.5 0 0 1 .314-.464L7.443.184z"/>
                                    </svg>
                                </div>
                                <h3 class="type-name text-truncate" title="${escapeHtmlAttribute(type.name)}">${escapeHtml(type.name)}</h3>
                            </div>
                            <div class="card-footer-actions">
                                <button type="button" class="btn-action-main edit-type-btn" data-id="${type.id}">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="currentColor" viewBox="0 0 16 16"><path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168l10-10zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207 11.207 2.5zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293l6.5-6.5zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325z"/></svg>
                                    Editar
                                </button>
                                <button type="button" class="btn-action-delete delete-type-btn" data-id="${type.id}" data-name="${escapeHtmlAttribute(type.name)}" title="Eliminar">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" fill="currentColor" viewBox="0 0 16 16"><path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/><path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/></svg>
                                </button>
                            </div>
                        </div>
                    </div>
                `);
            });
        });
    }

    $('#NewMachineTypeButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    $(document).on('click', '.edit-type-btn', function (e) {
        e.preventDefault();
        editModal.open({ id: $(this).data('id') });
    });

    $(document).on('click', '.delete-type-btn', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var name = $(this).data('name');

        abp.message.confirm(
            '¿Seguro que deseas eliminar el tipo de máquina "' + name + '"?',
            'Eliminar'
        ).then(function (confirmed) {
            if (confirmed) {
                service.delete(id).then(function () {
                    abp.notify.success('Tipo eliminado correctamente');
                    loadMachineTypes(); // Recargamos las tarjetas
                });
            }
        });
    });

    // En vez de recargar toda la página (F5), repintamos las tarjetas
    createModal.onResult(function () { loadMachineTypes(); });
    editModal.onResult(function () { loadMachineTypes(); });

    // Filtro buscador instantáneo (lado cliente)
    $("#typeSearchInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".machine-type-wrapper").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // Carga inicial
    loadMachineTypes();
});