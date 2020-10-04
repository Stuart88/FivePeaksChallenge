

window.downloadRouteData = function (fileType) {
    var link = document.createElement('a');
    link.href = `/Site/GetRouteData/${this.encodeURIComponent(fileType)}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.downloadLeaderboardRouteData = function (entryId) {
    var link = document.createElement('a');
    link.href = `/Leaderboard/GetRouteData/${entryId}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}


window.toggleWeatherWidget = function (show) {

    let weatherWidgetHTML = document.getElementById("weather-widget").innerHTML;

    let widgetArea = document.getElementById("widget-area");

    widgetArea.innerHTML = weatherWidgetHTML;

}

var quill;

window.prepareEditor = function () {
    var toolbarOptions = [
        ['bold', 'italic', 'underline', 'strike'], // toggled buttons
        ['blockquote', 'code-block'],
        [{ 'header': 1 }, { 'header': 2 }], // custom button values
        [{ 'list': 'ordered' }, { 'list': 'bullet' }],
        [{ 'script': 'sub' }, { 'script': 'super' }], // superscript/subscript
        [{ 'indent': '-1' }, { 'indent': '+1' }], // outdent/indent
        /*[{ 'direction': 'rtl' }], */ // text direction
        [{ 'size': ['small', false, 'large', 'huge'] }], // custom dropdown
        [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
        ['link', 'image' /*, 'video', 'formula'*/], // add's image support
        [{ 'color': [] }, { 'background': [] }], // dropdown with defaults from theme
        //[{ 'font': [] }],
        [{ 'align': [] }],
        ['clean'] // remove formatting button
    ];

    quill = new Quill('#quill-editor',
        {
            modules: {
                toolbar: toolbarOptions
            },

            theme: 'snow'
        });
}

window.getEditorText = function () {
    return quill.root.innerHTML;
}

window.setEditorText = function (text) {
    quill.root.innerHTML = text;
}
