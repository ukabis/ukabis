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
  // check input max value
  scoreInput.on('input', function (e) {
    e.preventDefault();
    const minPoint = parseInt($(this).attr('min'));
    const maxPoint = parseInt($(this).attr('max'));
    const currentValue = parseInt(e.target.value);
    if (typeof minPoint === 'number' && typeof maxPoint === 'number') {
      if (!currentValue && currentValue != minPoint) {
        $(this).val('');
        return false;
      }
      if (currentValue >= minPoint && currentValue <= maxPoint) {
        return $(this).val(currentValue);
      } else {
        return $(this).val(e.target.value.slice(0, -1));
      }
    }
  });

  let main = $('#checker_restaurant');
  let total = 0;

  function getMaxScore(max, scoreArr) {
    let total = 0

    $.each(scoreArr, (index, scoreData) => {
        let text = $(scoreData).find("p").text();
        let score = text.slice(0,-1);
        total += parseInt(score);
    });

    return total <= max ? total : max;
  }

  let scoreMenuQuestion1 = main.find('.type-menu-question-1');
  let maxMenuQuestion1 = getMaxScore(25, scoreMenuQuestion1);

  let scoreIngredientQuestion1 = main.find('.type-ingredient-question-1');
  let maxIngredientQuestion1 = getMaxScore(21, scoreIngredientQuestion1);

  let scoreQuestion2 = main.find('.type-checkbox-question-2');
  let scoreQuestion3 = main.find('.type-checkbox-question-3');
  let scoreQuestion2And3 = [...scoreQuestion2, ...scoreQuestion3];
  let maxQuestion2And3 = getMaxScore(20, scoreQuestion2And3);

  let scoreQuestion4 = main.find('.type-checkbox-question-4');
  let scoreQuestion5 = main.find('.type-checkbox-question-5');
  let scoreQuestion6 = main.find('.type-checkbox-question-6');
  let scoreMenuQuestion7 = main.find('.type-menu-question-7');
  let scoreIngredientQuestion7 = main.find('.type-ingredient-question-7');
  let scoreQuestion4To7 = [...scoreQuestion4, ...scoreQuestion5, ...scoreQuestion6, ...scoreMenuQuestion7, ...scoreIngredientQuestion7];
  let maxQuestion4To7 = getMaxScore(24, scoreQuestion4To7);

  let scoreQuestion8 = main.find('.type-checkbox-question-8');
  let maxQuestion8 = getMaxScore(10, scoreQuestion8);

  total = maxMenuQuestion1 + maxIngredientQuestion1 + maxQuestion2And3 + maxQuestion4To7 + maxQuestion8;

  main.find('.score_total_sec .total_score').text(total);
  main.find('.score_total_sec .input_total_score').val(total);
});
