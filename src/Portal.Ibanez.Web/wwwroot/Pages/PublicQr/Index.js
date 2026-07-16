document.addEventListener('DOMContentLoaded', function () {

    const qrCodeElement = document.getElementById('QrCode');
    const explorerContent = document.getElementById('ExplorerContent');
    const currentFolderName = document.getElementById('CurrentFolderName');
    const backButton = document.getElementById('BackButton');

    if (!qrCodeElement || !explorerContent) {
        return;
    }

    const qrCode = qrCodeElement.value;
    const folderHistory = [];

    async function loadExplorer(folderId, addToHistory) {

        let url =
            '/PublicQr/Explorer?code=' +
            encodeURIComponent(qrCode);

        if (folderId) {
            url +=
                '&folderId=' +
                encodeURIComponent(folderId);
        }

        explorerContent.innerHTML =
            '<div class="loading">Cargando documentación...</div>';

        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json'
                },
                credentials: 'same-origin'
            });

            if (!response.ok) {
                throw new Error(
                    'Error HTTP ' + response.status
                );
            }

            const result = await response.json();

            if (addToHistory && result.currentFolderId) {
                folderHistory.push(result.currentFolderId);
            }

            renderExplorer(result);
        }
        catch (error) {
            console.error(
                'Error cargando el explorador público:',
                error
            );

            explorerContent.innerHTML = `
                <div class="message error">
                    No se pudo cargar la documentación asociada al código QR.
                </div>
            `;
        }
    }

    function renderExplorer(result) {

        currentFolderName.textContent =
            result.currentFolderName || 'Documentación';

        backButton.style.display =
            folderHistory.length > 1
                ? 'inline-block'
                : 'none';

        const folders = result.folders || [];
        const documents = result.documents || [];

        if (folders.length === 0 &&
            documents.length === 0) {

            explorerContent.innerHTML = `
                <div class="message">
                    No hay documentación disponible.
                </div>
            `;

            return;
        }

        const itemsContainer = document.createElement('div');
        itemsContainer.className = 'items';

        folders.forEach(function (folder) {

            const button = document.createElement('button');

            button.type = 'button';
            button.className = 'item folder-item';
            button.dataset.id = folder.id;

            const title = document.createElement('div');
            title.className = 'item-title';
            title.textContent = '📁 ' + folder.name;

            const subtitle = document.createElement('div');
            subtitle.className = 'item-subtitle';
            subtitle.textContent =
                folder.description || 'Abrir carpeta';

            button.appendChild(title);
            button.appendChild(subtitle);

            button.addEventListener('click', function () {
                loadExplorer(folder.id, true);
            });

            itemsContainer.appendChild(button);
        });

        documents.forEach(function (documentItem) {

            const link = document.createElement('a');

            link.className = 'item';
            link.href =
                '/q/' +
                encodeURIComponent(qrCode) +
                '/download/' +
                encodeURIComponent(documentItem.id);

            const title = document.createElement('div');
            title.className = 'item-title';
            title.textContent =
                '📄 ' + documentItem.title;

            const subtitle = document.createElement('div');
            subtitle.className = 'item-subtitle';
            subtitle.textContent = documentItem.fileName;

            link.appendChild(title);
            link.appendChild(subtitle);

            itemsContainer.appendChild(link);
        });

        explorerContent.innerHTML = '';
        explorerContent.appendChild(itemsContainer);
    }

    backButton.addEventListener('click', function () {

        if (folderHistory.length <= 1) {
            return;
        }

        folderHistory.pop();

        const previousFolderId =
            folderHistory[folderHistory.length - 1];

        loadExplorer(previousFolderId, false);
    });

    loadExplorer(null, true);
});