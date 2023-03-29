$(function() {
    const loader = $('#loader');
    // リンククリック時にローディング画面を表示するイベントを追加する
    $('.JSLoderBtn').click(function() {
      // ローディング画面を表示するためのCSS
    setTimeout(function(){
        loader.css('display', 'block');
        }, 0);
    });
});