/*
 * 拡張の追加
 */
(function() {
    // marked.js + highlight.js
    var renderer = new marked.Renderer()

    // code syntax hilightの編集
    renderer.code = function (code) {
        return '<pre' + ' style="background: #1e1e1e; max-height: 500px;"><code class="hljs" >' + hljs.highlightAuto(code).value + '</code></pre>';
    };
    marked.setOptions({
        renderer: renderer,
    });
})();

/*
 * コードのプレビュー表示
 */
function convertMarkdownToHtml(target) {
    return marked(target, { sanitize: true, breaks: true });
}

/*
 * MarkdownをHTMLとして表示する。HTMLは指定した要素に埋め込まれる。
 */
function previewMarkdownAsHtml(target, elementId) {
    document.getElementById(elementId).innerHTML = convertMarkdownToHtml(target);
    console.log(document.getElementById(elementId));
}