$(document).everyTime(
    30000,
    function () {
        CheckImports()
    },
    0
);

function CheckImports() {
    $.ajax({
        type: "GET",
        url: "CheckImports?puth=" + document.getElementById("DataBackground").value,
        statusCode: {
            200: function () {
                document.location.href = "/Import/Message?info=Выгрузка%20прошла%20успешно";
            },
            409: function () {
                document.location.href = "/Import/Message?info=В%20процессе%20выгрузки%20произошла%20ошибка";
            }
        }
        
    });
};