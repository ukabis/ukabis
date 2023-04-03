//ヘッダーのページ一覧ホバーでメニュー表示
$(function () {
  const scoreInput = $('.score_input input.input-sc');

  $("p.menu a").mouseover(function () {
    $("header .menu_list").addClass("active");
  });
  $("header .menu_list ul li").mouseover(function () {
    $("header .menu_list").addClass("active");
  });
  $("p.menu a").mouseleave(function () {
    $("header .menu_list").removeClass("active");
  });
  $("header .menu_list ul li").mouseleave(function () {
    $("header .menu_list").removeClass("active");
  });
  $('.pic_block').click(function (e) {
    let id = $(e.target).attr('id');
    if (id) {
      initImageViewer(id, true)
    }
  });
});
