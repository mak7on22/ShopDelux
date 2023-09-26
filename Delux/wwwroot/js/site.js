﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Функция для отображения звездочек в зависимости от оценки
    $("#reviewForm").submit(function (e) {
        e.preventDefault(); // Отменяет стандартное действие формы

        // Получите данные формы
        var formData = {
            Rating: $("input[name='rating']:checked").val(),
            Comment: $("#Comment").val(),
            Author: $("#Author").val()
        };

        // Получите идентификатор телефона
        var phoneId = $(this).data("phone-id");
        location.reload(true);
        // Отправьте AJAX-запрос
        $.ajax({
            url: '/Reviews/AddReview/' + phoneId,
            type: 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            success: function (data) {
                // Обработка успешного ответа от сервера
                console.log(data);

                // Добавьте оценку к списку отзывов
                var rating = formData.Rating;
                var comment = formData.Comment;
                var author = formData.Author;
                //var reviewListItem = '<li class="liss"><strong>Автор:</strong> ' + author + '<br /><strong>Оценка:</strong> ' + rating + '<br /><strong>Комментарий:</strong> ' + comment + '</li>';
                $(".uls").append(reviewListItem);
                // Перезагрузите текущую страницу
                location.reload(true);
                // Дополнительная обработка, если необходимо
            },
            error: function (error) {
                // Обработка ошибки
                console.error(error);
            }
        });
    });

    // Обработчик изменения выбранной звездочки
    $("input[name='rating']").change(function () {
        var selectedRating = $(this).data("rating");

        // Здесь вы можете сделать что-то с выбранной оценкой, например, отобразить ее где-то в списке отзывов
        // В этом примере, мы просто выведем ее в консоль
        console.log("Выбранная оценка: " + selectedRating);
    });
});
$("#FoundationDate").on("change", function () {
    var foundationDate = new Date($(this).val());
    var currentDate = new Date();
    var minDate = new Date(currentDate);
    minDate.setFullYear(currentDate.getFullYear() - 100);

    if (foundationDate < minDate || foundationDate > currentDate) {
        alert("Дата основания должна быть не более чем 100 лет назад.");
        $(this).val(""); // Очищаем поле ввода, если дата недопустима
    } else {
        // дата удовлетворяет условиям, отправляем AJAX-запрос
        $.ajax({
            url: '/Validation/ValidateFoundationDate',
            type: 'POST',
            data: { foundationDate: foundationDate },
            success: function (response) {
                if (response !== true) {
                    // Ошибка - выводим сообщение
                    response;
                }
            }
        });
    }
});

$(document).ready(function () {
    $("#searchInput").on("keyup", function () {
        var searchTerm = $(this).val();
        console.log("Search term:", searchTerm);
        setTimeout(function () {
            $.ajax({
                url: '/Products/Search/',
                data: { term: searchTerm },
                method: "GET",
                success: function (data) {
                    console.log("Search results:", data);
                    $("#searchResults").empty();
                    if (data.length > 0) {
                        var products = data.split(';');
                        $.each(products, function (index, product) {
                            var productData = product.split(',');

                            var button = $("<button>")
                                .addClass("searh")
                                .text(productData[1])
                                .click(function () {
                                    console.log("Clicked Product:", productData);
                                    window.location.href = '/Products/Details/' + productData[0]; // Используем ProductId
                                });
                            $("#searchResults").append(button);
                        });
                    } else {
                        $("#searchResults").append("<p class='resu'>Нет результатов.</p>");
                    }
                }
            });
        },30);
    });
});


