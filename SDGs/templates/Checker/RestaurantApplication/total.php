<?php

/**
 * CakePHP(tm) : Rapid Development Framework (https://cakephp.org)
 * Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 *
 * Licensed under The MIT License
 * For full copyright and license information, please see the LICENSE.txt
 * Redistributions of files must retain the above copyright notice.
 *
 * @copyright Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 * @link      https://cakephp.org CakePHP(tm) Project
 * @since     0.10.0
 * @license   https://opensource.org/licenses/mit-license.php MIT License
 * @var \App\View\AppView $this
 */

use Cake\Core\Configure;

$count = 0;

?>
<?php $this->assign('title', '飲食店 設問・画像 合計点ページ'); ?>
<article class="contents" id="checker_restaurant">
  <section class="title_sec">
    <h2>飲食店 設問・画像 合計点ページ</h2>
  </section>

  <section class="list_sec">
    <table class="table-responsive">
      <tr>
        <th>回答ID</th>
        <th>代表者名</th>
        <th>屋号</th>
        <th>回答年月日日時</th>
        <th>ステータス</th>
      </tr>
      <tr>
        <td><?= $restaurantQuestion->id ?></td>
        <td><?= $this->CustomHtml->renderTextWithMaxLength($restaurantQuestion->office->business->representative_name) ?></td>
        <td><?= $this->CustomHtml->renderTextWithMaxLength($restaurantQuestion->office->name) ?></td>
        <td><?= $restaurantQuestion->modified_at->format('d/m/y h:i A') ?></td>
        <td class="no-wrap"><?= $restaurantQuestion->status_label ?></td>
      </tr>
    </table>
  </section>
  <?= $this->Form->create($restaurantQuestion); ?>
  <section class="score_total_sec">
    <p>
        <span class="total_score_title">合計点数</span>
        <span class="total_score"></span>
        <span class="total_score_unit">点</span>
    </p>
    <?= $this->Form->hidden('total', ['value' => '', 'class' => 'input_total_score']); ?>
  </section>

  <section class="score_marking_sec">
    <div class="tab_sec">
      <input id="tab_01" type="radio" name="tab_item">
      <label for="tab_01" class="tab_item first-child">登録店舗</label>
      <input id="tab_02" type="radio" name="tab_item" checked>
      <label for="tab_02" class="tab_item">設問／採点</label>
      <!-- #tab_01_content -->
      <div class="tab_content" id="tab_01_content">
        <table class="row_table">
          <tr>
            <th>業種</th>
            <td>飲食</td>
          </tr>
          <tr>
            <th>企業名</th>
            <td><?= h($restaurantQuestion->office->business->name) ?></td>
          </tr>
          <tr>
            <th>企業名（カナ）</th>
            <td><?= h($restaurantQuestion->office->business->name_kana) ?></td>
          </tr>
          <tr>
            <th>代表取締役</th>
            <td><?= h($restaurantQuestion->office->business->representative_name) ?></td>
          </tr>
          <tr>
            <th>郵便番号</th>
            <td><?= h($restaurantQuestion->office->zip_code) ?></td>
          </tr>
          <tr>
            <th>住所</th>
            <td><?= h($restaurantQuestion->office->prefecture) . ' '
                  . h($restaurantQuestion->office->city) . ' '
                  . h($restaurantQuestion->office->street) ?></td>
          </tr>
          <tr>
            <th>電話番号</th>
            <td><?= h($restaurantQuestion->office->tel) ?></td>
          </tr>
          <tr>
            <th>FAX番号</th>
            <td><?= h($restaurantQuestion->office->fax) ?></td>
          </tr>
          <tr>
            <th>店舗名</th>
            <td><?= h($restaurantQuestion->office->name) ?></td>
          </tr>
          <tr>
            <th>店舗名（カナ）</th>
            <td><?= h($restaurantQuestion->office->name_kana) ?></td>
          </tr>
        </table>
      </div>
      <!-- / #tab_01_content -->

      <!-- #tab_03_content -->
      <?php $restaurantQuestions = Configure::read('restaurantQuestionsScore'); ?>
      <div class="tab_content" id="tab_02_content">
        <?php foreach ($restaurantQuestions as $question => $option) : ?>
          <?php if ($option['type'] == TYPE_MULTI_MENU) : ?>
            <div class="score_marking_box b-none">
              <div class="no-flex_q_text <?= empty($answerQuestions["question_{$question}"]['menus']) ? 'd-flex-between pr25' : '' ?>">
                <p class="q_text">
                  <span class="q_num"><?= "0$question" ?></span>
                  <span><?= $option['title'] ?></span>
                </p>
                <?php if (empty($answerQuestions["question_{$question}"]['menus'])) : ?>
                    <div class="disp_score <?= "type-menu-question-{$question}" ?>">
                        <p>0点</p>
                    </div>
                <?php endif; ?>
              </div>
              <p class="q_text_comment"><?= !empty($option['sub_title_1']) ? $option['sub_title_1'] : '' ?></p>
              <p class="q_text_comment mb10"><?= !empty($option['sub_title_2']) ? $option['sub_title_2'] : '' ?></p>
              <div class="wrap_menu_block">
                <?php if (!empty($answerQuestions["question_{$question}"]) && !empty($answerQuestions["question_{$question}"]['menus'])) : ?>
                  <?php foreach ($answerQuestions["question_{$question}"]["menus"] as $menu => $answer) : ?>
                    <?php $menu = $menu + 1; ?>
                    <div class="score_marking_box">
                      <div class="flex_qa-btn-score">
                        <div class="qa_text total_width">
                          <p class="q_text_sub"><?= "メニュー名" . $menu ?></p>
                          <p class="a_text mb0"><?= $answer['name'] ?></p>
                        </div>
                        <div class="disp_score <?= "type-menu-question-{$question}" ?>">
                          <?php $score = json_decode($restaurantQuestion->{"question_{$question}_menu_{$menu}_score"}, true)['score']; ?>
                          <p><?= $score ? "{$score}点" : "0点" ?></p>
                        </div>
                      </div>
                    </div>
                    <?php if ($question === 1) : ?>
                        <?php
                            $localIngredients = array_filter($answer['ingredients'], fn($ingredient) => $ingredient['local_ingredient'] == true);
                            $bonusLocalIngredients = count($localIngredients) >= 2 ? 5 : 0;
                            if ($bonusLocalIngredients == 5) {
                                $count++;
                            }
                        ?>
                      <div class="score_marking_box">
                        <div class="flex_qa-btn-score">
                          <div class="qa_text total_width">
                            <p class="q_text_sub"><?= "メニュー{$menu}" ?> 地元産食材の使用について</p>
                            <p class="a_text">地元産食材を2品以上使用しているか</p>
                          </div>
                          <div class="disp_score <?= isset($count) && $count == 1 ? "type-menu-question-{$question}" : '' ?>">
                          <?php if ($bonusLocalIngredients == 5): ?>
                            <p>5点</p>
                          <?php else: ?>
                            <p>0点</p>
                          <?php endif; ?>
                          </div>
                        </div>
                      </div>
                    <?php endif; ?>
                    <?php foreach ($answer['ingredients'] as $index => $ingredient) : ?>
                      <?php foreach ($ingredient['extra_information'] as $type => $infos) : ?>
                        <?php if (!empty($option['menus'][$index + 1]['ingredients'][1]['extra_information'][$type])) : ?>
                          <?php $typeIngredient = $option['menus'][$index + 1]['ingredients'][1]['extra_information'][$type]; ?>
                          <?php foreach ($infos as $key => $info) : ?>
                            <div class="score_marking_box">
                              <div class="flex_qa-btn-score">
                                <div class="qa_text total_width">
                                  <?php if ($key == 0) : ?>
                                    <p class="q_text_sub"><?= "メニュー{$menu}" ?> 食材 <?= $typeIngredient['title'] ?></p>
                                  <?php endif; ?>
                                  <p class="a_text"><?= $typeIngredient['options'][$info]['label'] ?></p>
                                </div>
                                <div class="disp_score <?= "type-ingredient-question-{$question}" ?>">
                                  <p><?= $typeIngredient['options'][$info]['score'] ?>点</p>
                                </div>
                              </div>
                            </div>
                          <?php endforeach; ?>
                        <?php endif; ?>
                      <?php endforeach; ?>
                    <?php endforeach; ?>
                  <?php endforeach; ?>
                <?php endif; ?>
              </div>
          <?php elseif ($option['type'] == TYPE_CHECKBOX) : ?>
            <div class="score_marking_box b-none">
              <div class="no-flex_q_text space_bottom <?= empty($answerQuestions["question_{$question}"]['answers']) ? 'd-flex-between pr25' : '' ?>">
                <p class="q_text">
                  <span class="q_num"><?= "0$question" ?></span>
                  <span><?= $option['title'] ?></span>
                </p>
                <?php if (empty($answerQuestions["question_{$question}"]['answers'])): ?>
                  <div class="disp_score <?= "type-checkbox-question-{$question}" ?>">
                    <p>0点</p>
                  </div>
                <?php endif; ?>
              </div>
              <?php if (!empty($answerQuestions["question_{$question}"]) && !empty($answerQuestions["question_{$question}"]['answers'])) : ?>
                <?php foreach ($answerQuestions["question_{$question}"]['answers'] as $key => $answer) : ?>
                  <div class="score_marking_box">
                    <div class="flex_qa-btn-score">
                      <div class="qa_text total_width">
                        <p class="q_text_sub"></p>
                        <p class="a_text"><?= $option['options'][$answer]['label'] ?></p>
                      </div>
                      <div class="disp_score <?= "type-checkbox-question-{$question}" ?>">
                        <p><?= $option['options'][$answer]['score'] ?>点</p>
                      </div>
                    </div>
                  </div>
                <?php endforeach; ?>
              <?php endif; ?>
            </div>
          <?php endif; ?>
        <?php endforeach; ?>
        </div>
      </div>
  </section>

  <section class="btn_sec">
    <?= $this->Form->submit('飲食店 メール送信ページへ') ?>
    <p><button class="backBtn">
        <?= $this->Html->link('飲食店・画像設問採点ページへ戻る', $this->Url->build([
          'prefix' => 'Checker',
          'controller' => 'RestaurantApplication',
          'action' => 'score',
          'id' => $restaurantQuestion->id
        ], ['fullBase' => true])) ?>
      </button></p>
  </section>
  <?= $this->Form->end(); ?>
</article>
