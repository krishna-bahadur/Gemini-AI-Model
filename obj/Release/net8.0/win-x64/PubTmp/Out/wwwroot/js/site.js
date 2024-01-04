function scrollToSection() {
    var targetElement = document.querySelector('.AI_section');

    if (targetElement) {
        targetElement.scrollIntoView({ behavior: 'smooth' });
    }
}

function displayImage(input) {
    var img = document.getElementById('uploadedImage');

    $('#Description').val('');

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            img.src = e.target.result;
            img.width = 200; 
            img.height = 200; 
            img.style.objectFit = 'cover';
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function AppendContent(button) {
    var buttonText = button.textContent || button.innerText;
    $('#Description').val(buttonText);
}


$(document).ready(function () {
    // Handle form submission
    $('#aiForm').submit(function (event) {

        $('.response').empty('');
        $('#lds-roller').show();
        $('.ai-reposne').hide();


        event.preventDefault();

        var formData = new FormData($(this)[0]);

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: formData,
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.success) {
                    simulateTyping(result.message, $('.response'));
                }
            },
            error: function (error) {
                console.error('Error submitting form', error);
            },
            complete: function () {
                $('#lds-roller').hide();
                $('.ai-reposne').show();
            }
        });
    });
});

function simulateTyping(text, targetElement) {
    var index = 0;

    function addNextCharacter() {
        var char = text[index];
        if (char === '\n') {
            targetElement.append('<br>');
        } else {
            targetElement.append(char);
        }
        index++;

        if (index < text.length) {
            setTimeout(addNextCharacter, 20); 
        }
    }

    addNextCharacter();
}