$(function () {

    var service = portal.ibanez.documentFolders.documentFolder;

    var createDocumentModal = new abp.ModalManager({
        viewUrl: '/MachineDocuments/CreateModal'
    });

    var uploadFolderModal = new abp.ModalManager({
        viewUrl: '/MachineDocuments/UploadFolderModal'
    });


    var editModal = new abp.ModalManager({
        viewUrl: '/DocumentFolders/EditModal'
    });

    function loadExplorer() {

        var machineId = $('#MachineId').val();
        var parentFolderId = $('#ParentFolderId').val();

        service.getExplorer({
            machineId: machineId,
            parentFolderId: parentFolderId || null
        }).then(function (result) {

            var folders = result.items || [];
            var documents = result.documents || [];


            toggleButtons(result);

            var container = $('#DocumentFoldersExplorer');
            container.empty();

            if (folders.length === 0 && documents.length === 0) {
                container.append(`
                    <div class="col-12">
                        <div class="alert alert-light border text-center text-muted mb-0">
                            No hay carpetas disponibles en esta ubicación.
                        </div>
                    </div>
                `);

                return;
            }

            folders.forEach(function (folder) {

                var countText;

                if (folder.subFoldersCount > 0) {
                    countText = folder.subFoldersCount === 1
                        ? '1 subcarpeta'
                        : folder.subFoldersCount + ' subcarpetas';
                }
                else if (folder.documentsCount > 0) {
                    countText = folder.documentsCount === 1
                        ? '1 documento'
                        : folder.documentsCount + ' documentos';
                }
                else {
                    countText = 'Carpeta vacía';
                }

                var statusBadge = folder.isActive
                    ? '<span class="badge bg-success">Activa</span>'
                    : '<span class="badge bg-secondary">Inactiva</span>';

                var description = folder.description
                    ? `<p class="text-muted small mb-3">${escapeHtml(folder.description)}</p>`
                    : '<p class="text-muted small mb-3">Sin descripción</p>';

                container.append(`
                    <div class="col">
                        <div class="card h-100 shadow-sm border folder-card">
                            <div class="card-body d-flex flex-column">

                                <div class="d-flex justify-content-between align-items-start mb-3">
                                    <div class="folder-icon">
                                        <i class="fa fa-folder fa-2x text-warning"></i>
                                    </div>

                                    ${statusBadge}
                                </div>

                                <h5 class="card-title mb-2">
                                    ${escapeHtml(folder.name)}
                                </h5>

                                ${description}

                                <div class="text-muted small mb-3">
                                    ${countText}
                                </div>

                                <div class="mt-auto d-flex gap-2">

                                    <button type="button"
                                            class="btn btn-primary btn-sm flex-grow-1 open-folder-btn"
                                            data-id="${folder.id}">
                                        Abrir
                                    </button>

                                    <div class="dropdown">
                                        <button type="button"
                                                class="btn btn-outline-secondary btn-sm dropdown-toggle"
                                                data-bs-toggle="dropdown"
                                                aria-expanded="false">
                                            Acciones
                                        </button>

                                        <ul class="dropdown-menu dropdown-menu-end">

                                            <li>
                                                <button type="button"
                                                        class="dropdown-item edit-folder-btn"
                                                        data-id="${folder.id}">
                                                    Editar
                                                </button>
                                            </li>

                                            <li>
                                                <hr class="dropdown-divider">
                                            </li>

                                            <li>
                                                <button type="button"
                                                        class="dropdown-item text-danger delete-folder-btn"
                                                        data-id="${folder.id}"
                                                        data-name="${escapeHtmlAttribute(folder.name)}">
                                                    Eliminar
                                                </button>
                                            </li>

                                        </ul>
                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>
                `);
            });
            documents.forEach(function (doc) {

                container.append(`
        <div class="col">
            <div class="card h-100 shadow-sm border">

                <div class="card-body d-flex flex-column">

                    <div class="mb-3">
                        <i class="fa fa-file-pdf fa-2x text-danger"></i>
                    </div>

                    <h5 class="card-title">
                        ${escapeHtml(doc.title)}
                    </h5>

                    <p class="text-muted small mb-3">
                        ${escapeHtml(doc.fileName)}
                    </p>

                    <div class="mt-auto">

                        <a class="btn btn-success btn-sm w-100"
                           href="/MachineDocuments/Download/${doc.id}">
                            Descargar
                        </a>

                    </div>

                </div>

            </div>
        </div>
    `);

            });
        });
    }
    $('#NewDocumentButton').click(function (e) {
        e.preventDefault();

        var machineId = $('#MachineId').val();
        var documentFolderId = $('#ParentFolderId').val();

        if (!documentFolderId) {
            abp.notify.warn('Debes entrar en una carpeta antes de añadir documentos.');
            return;
        }

        createDocumentModal.open({
            machineId: machineId,
            documentFolderId: documentFolderId
        });
    });

    $('#UploadFolderButton').click(function (e) {
        e.preventDefault();

        var machineId = $('#MachineId').val();
        var documentFolderId = $('#ParentFolderId').val();

        if (!documentFolderId) {
            abp.notify.warn('Debes entrar en una carpeta antes de subir documentos.');
            return;
        }

        uploadFolderModal.open({
            machineId: machineId,
            documentFolderId: documentFolderId
        });
    });
    $('#NewDocumentFolderButton').click(function (e) {
        e.preventDefault();

        var machineId = $('#MachineId').val();
        var parentFolderId = $('#ParentFolderId').val();

        var modalUrl =
            '/DocumentFolders/CreateModal?machineId=' +
            encodeURIComponent(machineId);

        if (parentFolderId) {
            modalUrl +=
                '&parentFolderId=' +
                encodeURIComponent(parentFolderId);
        }

        var folderModal = new abp.ModalManager({
            viewUrl: modalUrl
        });

        folderModal.onResult(function () {
            location.reload();
        });

        folderModal.open();
    });

    $(document).on('click', '.open-folder-btn', function () {

        var folderId = $(this).data('id');
        var machineId = $('#MachineId').val();

        window.location.href =
            '/DocumentFolders?machineId=' +
            encodeURIComponent(machineId) +
            '&parentFolderId=' +
            encodeURIComponent(folderId);
    });

    $(document).on('click', '.edit-folder-btn', function () {

        editModal.open({
            id: $(this).data('id')
        });
    });

    $(document).on('click', '.delete-folder-btn', function () {

        var folderId = $(this).data('id');
        var folderName = $(this).data('name');

        abp.message.confirm(
            '¿Seguro que deseas eliminar la carpeta "' + folderName + '"?',
            'Eliminar carpeta'
        ).then(function (confirmed) {

            if (!confirmed) {
                return;
            }

            service.delete(folderId)
                .then(function () {
                    abp.notify.success(
                        'Carpeta eliminada correctamente'
                    );

                    loadExplorer();
                });
        });
    });

    editModal.onResult(function () {
        location.reload();
    });

    createDocumentModal.onResult(function () {
        location.reload();
    });

    uploadFolderModal.onResult(function () {
        location.reload();
    });

    function toggleButtons(result) {

        $('#NewDocumentFolderButton')
            .toggle(result.canCreateFolder);

        var newDocumentButton = $('#NewDocumentButton');

        if (newDocumentButton.length > 0) {
            newDocumentButton.toggle(
                result.canUploadDocuments
            );
        }

        var uploadFolderButton = $('#UploadFolderButton');

        if (uploadFolderButton.length > 0) {
            uploadFolderButton.toggle(
                result.canUploadFolder
            );
        }
    }

    function escapeHtml(value) {
        return $('<div>')
            .text(value || '')
            .html();
    }

    function escapeHtmlAttribute(value) {
        return String(value || '')
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }

    loadExplorer();
});