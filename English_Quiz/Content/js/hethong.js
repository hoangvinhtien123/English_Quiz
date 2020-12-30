var FormModalIsValid = true;
const loadingStart = () => $("body").addClass("loading");
const loadingStop = () => $("body").removeClass("loading");
function ValidateRequireControl(el, errorMes) {
    $(el).removeClass('is-invalid')
    $(el).nextAll(".spanError").remove()
    if ($(el).val() == null || $(el).val() == undefined) {
        $(el).addClass('is-invalid')
        $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
        FormModalIsValid = false
    }
    else {
        let valuectl = $(el).val().trim()
        if (valuectl == null || valuectl == '') {
            $(el).addClass('is-invalid')
            $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
            FormModalIsValid = false
        }
    }
}

function CreateValidate(el, errorMes) {
    $(el).removeClass('is-invalid')
    $(el).nextAll(".spanError").remove()

    $(el).addClass('is-invalid')
    $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
    FormModalIsValid = false
}

function ClearError(el) {
    $(el).removeClass('is-invalid')
    $(el).nextAll(".spanError").remove()
}

function ValidateRequireControlMaxLength(el, errorMes, length) {
    if (FormModalIsValid) {
        let valuectl = $(el).val().trim()
        if (valuectl != null && valuectl != '') {
            $(el).removeClass('is-invalid')
            $(el).nextAll(".spanError").remove()
            if (valuectl.length > length) {
                $(el).addClass('is-invalid')
                $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
                FormModalIsValid = false
            }
        }
    }
}
function ValidateRequireControlMinLength(el, errorMes, length) {
    if (FormModalIsValid) {
        let valuectl = $(el).val().trim()
        if (valuectl != null && valuectl != '') {
            $(el).removeClass('is-invalid')
            $(el).nextAll(".spanError").remove()
            if (valuectl.length < length) {
                $(el).addClass('is-invalid')
                $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
                FormModalIsValid = false
            }
        }
    }
}

function ValidateRequireControlPass(el1, el2, errorMes) {
    let valuectl1 = $(el1).val().trim()
    let valuectl2 = $(el2).val().trim()
    if (valuectl1 != null && valuectl1 != '' && valuectl2 != null && valuectl2 != '') {
        $(el2).removeClass('is-invalid')
        $(el2).nextAll(".spanError").remove()
        if (valuectl1 != valuectl2) {
            $(el2).addClass('is-invalid')
            $(el2).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
            FormModalIsValid = false
        }
    }
}

function ValidateRequireControlEmail(el, errorMes) {
    let valuectl = $(el).val().trim()
    $(el).removeClass('is-invalid')
    $(el).nextAll(".spanError").remove()
    if (FormModalIsValid) {
        if (ValidateEmail(valuectl) == false) {
            $(el).addClass('is-invalid')
            $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
            FormModalIsValid = false
        }
    }
}

function ValidateRequireControlDate(el, errorMes) {
    let valuectl = $(el).val().trim()
    $(el).removeClass('is-invalid')
    $(el).nextAll(".spanError").remove()
    if (FormModalIsValid) {
        if (checkDateTime(valuectl) == false) {
            $(el).addClass('is-invalid')
            $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
            FormModalIsValid = false
        }
    }
}

function ValidateNumber(el, errorMes) {
    let valuectl = $(el).val().trim()
    $(el).removeClass('is-invalid')
    $(el).nextAll(".spanError").remove()
    if (FormModalIsValid) {
        if (CheckIsNumber(valuectl) == false) {
            $(el).addClass('is-invalid')
            $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
            FormModalIsValid = false
        }
    }
}

function ValidatePassword(el, errorMes) {
    let valuectl = $(el).val().trim()
    $(el).removeClass('is-invalid')
    $(el).nextAll(".spanError").remove()
    if (FormModalIsValid) {
        if (CheckPassword(valuectl) == false) {
            $(el).addClass('is-invalid')
            $(el).after(`<span class = "text-danger spanError"> ${errorMes}</span>`)
            FormModalIsValid = false
        }
    }
}
function ValidateEmail(mail) {
    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
        return (true)
    }
    return (false)
}

// mật khẩu phải có 1 chữ hoa, 1 chữ thường , 1 số, 1 chữ cái đặc biệt, 8-15 kí tự
function CheckPassword(inputtxt) {
    let decimal = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{8,15}$/;
    if (inputtxt.match(decimal)) {
        return true
    }
    return false
}
function checkDateTime(input) {
    let dateRex = new RegExp(/^(?:(?:(?:0?[13578]|1[02])(\/|-|\.)31)\1|(?:(?:0?[1,3-9]|1[0-2])(\/|-|\.)(?:29|30)\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:0?2(\/|-|\.)29\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:(?:0?[1-9])|(?:1[0-2]))(\/|-|\.)(?:0?[1-9]|1\d|2[0-8])\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/)
    if (dateRex.test(input)) {
        return true;
    }
    return false;
}

function CheckIsNumber(num) {
    if (Number.isInteger(Number(num))) {
        return true
    }
    return false
}

function LoadAjaxContent(url, container) {
    $(container).html("<div><div class=\"loader\"></div></div>");
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "GET",
        success: function (data) {
            $(container).html(data);
        }
    });
}

async function LoadAjaxContentAsync(url, container) {
    $(container).html("<div><div class=\"loader\"></div></div>");
    var res = await $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "GET",
        success: function (data) {
            $(container).html(data);
        }
    });
    return res;
}
function PostAction(url, container) {
    $(container).html("<div><div class=\"loader\"></div></div>");
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "GET",
        success: function (data) {
            $(container).html(data);
        }
    });
}

function generateRandomFileName(filename) {
    return (
        Date.now() +
        '' +
        Math.floor(Math.random() * 1000000000) +
        '.' +
        filename.split('.').pop()
    );
}
async function UploadSimpleFile(elFile, fileName, folderName) {
    let formData = new FormData();
    let file = $(elFile)[0];
    formData.append('file', file.files[0], fileName);
    formData.append('folderName', folderName);
    await $.ajax({
        url: '/Admin/CommonAdminAdmin/UploadSimpleFile',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
        },
        error: function (err) {
        },
    })
}