$(function () {

    var service = portal.ibanez.qrCodes.qrCode;

    var createModal = new abp.ModalManager({
        viewUrl: '/QrCodes/Documents/CreateModal'
    });

    var dataTable = $('#QrCodeDocumentsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: false,
            paging: false,
            searching: false,
            scrollX: true,

            ajax: function (data, callback) {
                service.getDocuments($('#QrCodeId').val())
                    .then(function (result) {
                        callback({
                            recordsTotal: result.length,
                            recordsFiltered: result.length,
                            data: result
                        });
                    });
            },

            columnDefs: [
                {
                    rowAction: {
                        items: [
                            {
                                text: 'Quitar',
                                confirmMessage: function () {
                                    return '¿Seguro que deseas quitar este documento del QR?';
                                },
                                action: function (data) {
                                    service.removeDocument(data.record.id)
                                        .then(function () {
                                            abp.notify.success('Documento quitado del QR');
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    },
                    width: "80px"
                },
                {
                    title: 'Documento',
                    data: "documentTitle"
                },
                {
                    title: 'Archivo',
                    data: "fileName"
                },
                {
                    title: 'Orden',
                    data: "displayOrder"
                }
            ]
        })
    );

    $('#NewQrCodeDocumentButton').click(function (e) {
        e.preventDefault();

        createModal.open({
            qrCodeId: $('#QrCodeId').val()
        });
    });

    createModal.onResult(function () {
        location.reload();
    });

});