$(function () {
    // Servicio para las Carpetas
    var service = portal.ibanez.documentFolders.documentFolder;
    // Servicio para los Documentos (para poder eliminarlos)
    var docService = portal.ibanez.documents.machineDocument;

    var createDocumentModal = new abp.ModalManager({ viewUrl: '/MachineDocuments/CreateModal' });
    var uploadFolderModal = new abp.ModalManager({ viewUrl: '/MachineDocuments/UploadFolderModal' });
    var editModal = new abp.ModalManager({ viewUrl: '/DocumentFolders/EditModal' });

    // Helper para obtener iconos de colores según la extensión
    function getDocumentSvgIcon(fileName) {
        var ext = fileName ? fileName.split('.').pop().toLowerCase() : '';
        if (ext === 'pdf') {
            return { color: 'text-danger', svg: '<svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" fill="currentColor" viewBox="0 0 16 16"><path d="M14 14V4.5L9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2zM9.5 3A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5v2z"/><path d="M4.603 14.087a.81.81 0 0 1-.438-.42c-.195-.388-.13-.776.08-1.102.198-.307.526-.568.897-.787a7.68 7.68 0 0 1 1.482-.645 19.697 19.697 0 0 0 1.062-2.227 7.269 7.269 0 0 1-.43-1.295c-.086-.4-.119-.796-.046-1.136.075-.354.274-.672.65-.823.192-.077.4-.12.602-.077a.7.7 0 0 1 .477.365c.088.164.12.356.127.538.007.188-.012.396-.047.614-.084.51-.27 1.134-.52 1.794a10.954 10.954 0 0 0 .98 1.686 5.753 5.753 0 0 1 1.334.05c.364.066.734.195.96.465.12.144.193.32.2.518.007.192-.047.382-.138.563a1.04 1.04 0 0 1-.354.416.856.856 0 0 1-.51.138c-.331-.014-.654-.196-.933-.417a5.712 5.712 0 0 1-.911-.95 11.651 11.651 0 0 0-1.997.406 11.307 11.307 0 0 1-1.02 1.51c-.292.35-.609.656-.927.787a.793.793 0 0 1-.58.029zm1.379-1.901c-.166.076-.32.156-.459.238-.328.194-.541.383-.647.547-.094.145-.096.25-.04.361.01.022.02.036.026.044a.266.266 0 0 0 .035-.012c.137-.056.355-.235.635-.572a8.18 8.18 0 0 0 .45-.606zm1.64-1.33a12.71 12.71 0 0 1 1.01-.193 11.744 11.744 0 0 1-.51-.858 20.801 20.801 0 0 1-.5 1.05zm2.446.45c.15.163.296.3.435.41.24.19.407.253.498.256a.107.107 0 0 0 .07-.015.307.307 0 0 0 .094-.125.436.436 0 0 0 .059-.2.095.095 0 0 0-.026-.063c-.052-.062-.2-.152-.518-.209a3.876 3.876 0 0 0-.612-.053zM8.078 7.8a6.7 6.7 0 0 0 .2-.828c.031-.188.043-.343.038-.465a.613.613 0 0 0-.032-.198.517.517 0 0 0-.145.04c-.087.035-.158.106-.196.283-.04.192-.03.469.046.822.024.111.054.227.09.346z"/></svg>' };
        } else if (ext === 'xls' || ext === 'xlsx') {
            return { color: 'text-success', svg: '<svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" fill="currentColor" viewBox="0 0 16 16"><path d="M14 14V4.5L9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2zM9.5 3A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5v2z"/><path d="M5.884 6.68 8 9.219l2.116-2.54a.5.5 0 1 1 .768.641L8.651 10l2.233 2.68a.5.5 0 0 1-.768.64L8 10.781l-2.116 2.54a.5.5 0 0 1-.768-.641L7.349 10 5.116 7.32a.5.5 0 1 1 .768-.64z"/></svg>' };
        } else if (ext === 'doc' || ext === 'docx') {
            return { color: 'text-primary', svg: '<svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" fill="currentColor" viewBox="0 0 16 16"><path d="M14 14V4.5L9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2zM9.5 3A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5v2z"/><path d="M5.5 6.5A.5.5 0 0 1 6 6h4a.5.5 0 0 1 0 1H6a.5.5 0 0 1-.5-.5zm0 2A.5.5 0 0 1 6 8h4a.5.5 0 0 1 0 1H6a.5.5 0 0 1-.5-.5zm0 2A.5.5 0 0 1 6 10h4a.5.5 0 0 1 0 1H6a.5.5 0 0 1-.5-.5z"/></svg>' };
        } else if (ext === 'jpg' || ext === 'jpeg' || ext === 'png') {
            return { color: 'text-info', svg: '<svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" fill="currentColor" viewBox="0 0 16 16"><path d="M14 14V4.5L9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2zM9.5 3A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5v2z"/><path d="M4 10.5a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-7a.5.5 0 0 1-.5-.5v-1zm0-2a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-7a.5.5 0 0 1-.5-.5v-1zm0-2a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-7a.5.5 0 0 1-.5-.5v-1z"/></svg>' };
        } else {
            return { color: 'text-secondary', svg: '<svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" fill="currentColor" viewBox="0 0 16 16"><path d="M14 4.5V14a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2a2 2 0 0 1 2-2h5.5L14 4.5zm-3 0A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V4.5h-2z"/></svg>' };
        }
    }

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
                    <div class="col-12 text-center py-5 w-100">
                        <div class="bg-white p-5 rounded-4 border text-muted shadow-sm d-flex flex-column align-items-center">
                            <div class="mb-3 p-3 bg-light rounded-circle text-secondary">
                                <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" fill="currentColor" viewBox="0 0 16 16"><path d="M9.828 3h3.982a2 2 0 0 1 1.992 2.181l-.637 7A2 2 0 0 1 13.174 14H2.825a2 2 0 0 1-1.991-1.819l-.637-7a1.99 1.99 0 0 1 .342-1.31L.5 3a2 2 0 0 1 2-2h3.672a2 2 0 0 1 1.414.586l.828.828A2 2 0 0 0 9.828 3zm-8.322.12C1.72 3.042 1.95 3 2.19 3h5.396l-.707-.707A1 1 0 0 0 6.172 2H2.5a1 1 0 0 0-1 .981l.006.139z"/></svg>
                            </div>
                            <h4 class="h5 text-dark fw-bold mb-1">Carpeta vacía</h4>
                            <p class="mb-0 text-secondary" style="font-size: 0.9rem;">No hay subcarpetas ni documentos en esta ubicación.</p>
                        </div>
                    </div>
                `);
                return;
            }

            // INYECCIÓN DE CARPETAS
            folders.forEach(function (folder) {
                var countText;
                if (folder.subFoldersCount > 0) countText = folder.subFoldersCount === 1 ? '1 subcarpeta' : folder.subFoldersCount + ' subcarpetas';
                else if (folder.documentsCount > 0) countText = folder.documentsCount === 1 ? '1 documento' : folder.documentsCount + ' documentos';
                else countText = 'Carpeta vacía';

                var statusClass = folder.isActive ? 'status-active' : 'status-inactive';
                var statusText = folder.isActive ? 'Activa' : 'Inactiva';

                var desc = folder.description ? escapeHtml(folder.description) : 'Sin descripción';

                container.append(`
                    <div class="col">
                        <div class="modern-card h-100 shadow-sm">
                            <div class="card-body p-4 d-flex flex-column h-100">
                                
                                <div class="d-flex justify-content-between align-items-start mb-3">
                                    <div class="folder-icon-wrapper shadow-sm">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="26" height="26" fill="currentColor" viewBox="0 0 16 16"><path d="M9.828 3h3.982a2 2 0 0 1 1.992 2.181l-.637 7A2 2 0 0 1 13.174 14H2.825a2 2 0 0 1-1.991-1.819l-.637-7a1.99 1.99 0 0 1 .342-1.31L.5 3a2 2 0 0 1 2-2h3.672a2 2 0 0 1 1.414.586l.828.828A2 2 0 0 0 9.828 3zm-8.322.12C1.72 3.042 1.95 3 2.19 3h5.396l-.707-.707A1 1 0 0 0 6.172 2H2.5a1 1 0 0 0-1 .981l.006.139z"/></svg>
                                    </div>
                                    <span class="status-badge ${statusClass}">${statusText}</span>
                                </div>

                                <h3 class="h6 fw-bold text-dark mb-1 text-truncate" title="${escapeHtmlAttribute(folder.name)}">${escapeHtml(folder.name)}</h3>
                                <p class="text-muted small mb-3 text-truncate" title="${escapeHtmlAttribute(desc)}">${desc}</p>
                                
                                <div class="text-muted small mt-auto mb-3 fw-medium d-flex align-items-center gap-1">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" fill="currentColor" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M1 8a7 7 0 1 0 14 0A7 7 0 0 0 1 8zm15 0A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM7.5 3a.5.5 0 0 1 .5.5v5.21l3.247 1.856a.5.5 0 0 1-.494.868l-3.5-2A.5.5 0 0 1 7 9V3.5a.5.5 0 0 1 .5-.5z"/></svg>
                                    ${countText}
                                </div>

                                <div class="d-flex gap-2">
                                    <button type="button" class="btn-action-main open-folder-btn" data-id="${folder.id}">
                                        Abrir
                                    </button>
                                    
                                    <div class="dropdown">
                                        <button class="btn-action-icon h-100" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" fill="currentColor" viewBox="0 0 16 16"><path d="M9.5 13a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z" /></svg>
                                        </button>
                                        <ul class="dropdown-menu dropdown-menu-end shadow-sm border-0 mt-1 p-2" style="font-size: 0.85rem; border-radius: 10px;">
                                            <li>
                                                <button type="button" class="dropdown-item d-flex align-items-center gap-2 edit-folder-btn py-2 rounded" data-id="${folder.id}">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="#64748b" viewBox="0 0 16 16"><path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168l10-10zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207 11.207 2.5zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293l6.5-6.5zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325z"/></svg>
                                                    Editar
                                                </button>
                                            </li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li>
                                                <button type="button" class="dropdown-item text-danger d-flex align-items-center gap-2 py-2 rounded delete-folder-btn" data-id="${folder.id}" data-name="${escapeHtmlAttribute(folder.name)}">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="currentColor" viewBox="0 0 16 16"><path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/><path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/></svg>
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

            // INYECCIÓN DE DOCUMENTOS CON BOTÓN ELIMINAR AÑADIDO
            documents.forEach(function (doc) {
                var iconData = getDocumentSvgIcon(doc.fileName);

                container.append(`
                    <div class="col">
                        <div class="modern-card h-100 shadow-sm" style="background-color: #fafbfc;">
                            <div class="card-body p-4 d-flex flex-column h-100">
                                
                                <div class="mb-3">
                                    <div class="doc-icon-wrapper ${iconData.color}">
                                        ${iconData.svg}
                                    </div>
                                </div>

                                <h3 class="h6 fw-bold text-dark mb-1 text-truncate" title="${escapeHtmlAttribute(doc.title)}">
                                    ${escapeHtml(doc.title)}
                                </h3>

                                <p class="text-muted small mb-3 text-truncate" title="${escapeHtmlAttribute(doc.fileName)}" style="font-size: 0.75rem;">
                                    ${escapeHtml(doc.fileName)}
                                </p>

                                <div class="d-flex gap-2 mt-auto">
                                    <a class="btn-action-main btn-action-download" href="/MachineDocuments/Download/${doc.id}" target="_blank">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" fill="currentColor" viewBox="0 0 16 16"><path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/><path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/></svg>
                                        Descargar
                                    </a>
                                    
                                    <div class="dropdown">
                                        <button class="btn-action-icon h-100" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" fill="currentColor" viewBox="0 0 16 16"><path d="M9.5 13a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z" /></svg>
                                        </button>
                                        <ul class="dropdown-menu dropdown-menu-end shadow-sm border-0 mt-1 p-2" style="font-size: 0.85rem; border-radius: 10px;">
                                            <li>
                                                <button type="button" class="dropdown-item text-danger d-flex align-items-center gap-2 py-2 rounded delete-doc-btn" data-id="${doc.id}" data-name="${escapeHtmlAttribute(doc.title)}">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="currentColor" viewBox="0 0 16 16"><path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/><path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/></svg>
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

        var modalUrl = '/DocumentFolders/CreateModal?machineId=' + encodeURIComponent(machineId);
        if (parentFolderId) {
            modalUrl += '&parentFolderId=' + encodeURIComponent(parentFolderId);
        }

        var folderModal = new abp.ModalManager({ viewUrl: modalUrl });
        folderModal.onResult(function () { location.reload(); });
        folderModal.open();
    });

    // Lógica para CARPETAS
    $(document).on('click', '.open-folder-btn', function () {
        var folderId = $(this).data('id');
        var machineId = $('#MachineId').val();

        window.location.href = '/DocumentFolders?machineId=' + encodeURIComponent(machineId) + '&parentFolderId=' + encodeURIComponent(folderId);
    });

    $(document).on('click', '.edit-folder-btn', function () {
        editModal.open({ id: $(this).data('id') });
    });

    $(document).on('click', '.delete-folder-btn', function () {
        var folderId = $(this).data('id');
        var folderName = $(this).data('name');

        abp.message.confirm(
            '¿Seguro que deseas eliminar la carpeta "' + folderName + '"?',
            'Eliminar carpeta'
        ).then(function (confirmed) {
            if (!confirmed) return;
            service.delete(folderId).then(function () {
                abp.notify.success('Carpeta eliminada correctamente');
                loadExplorer();
            });
        });
    });

    $(document).on('click', '.delete-doc-btn', function () {
        var docId = $(this).data('id');
        var docName = $(this).data('name');

        abp.message.confirm(
            '¿Seguro que deseas eliminar el documento "' + docName + '"?',
            'Eliminar documento'
        ).then(function (confirmed) {
            if (!confirmed) return;
            docService.delete(docId).then(function () {
                abp.notify.success('Documento eliminado correctamente');
                loadExplorer();
            });
        });
    });

    editModal.onResult(function () { location.reload(); });
    createDocumentModal.onResult(function () { location.reload(); });
    uploadFolderModal.onResult(function () { location.reload(); });

    function toggleButtons(result) {
        $('#NewDocumentFolderButton').toggle(result.canCreateFolder);
        var newDocumentButton = $('#NewDocumentButton');
        if (newDocumentButton.length > 0) newDocumentButton.toggle(result.canUploadDocuments);
        var uploadFolderButton = $('#UploadFolderButton');
        if (uploadFolderButton.length > 0) uploadFolderButton.toggle(result.canUploadFolder);
    }

    function escapeHtml(value) {
        return $('<div>').text(value || '').html();
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