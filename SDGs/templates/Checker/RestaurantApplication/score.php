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
<?php $this->assign('title', '飲食店 設問・画像採点ページ'); ?>
<article class="contents">
  <section class="title_sec">
    <h2>飲食店 設問・画像採点ページ</h2>
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
  <?= $this->Form->create(); ?>
  <section class="score_marking_sec">
    <div class="tab_sec">
      <input id="tab_01" type="radio" name="tab_item">
      <label for="tab_01" class="tab_item first-child">登録店舗</label>
      <input id="tab_03" type="radio" name="tab_item" checked>
      <label for="tab_03" class="tab_item">設問／採点</label>
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
      <?php
        $restaurantQuestions = Configure::read('restaurantQuestionsScore');
      ?>
      <div class="tab_content" id="tab_03_content">
        <?php foreach ($restaurantQuestions as $question => $option) : ?>
          <?php if ($option['type'] == TYPE_MULTI_MENU) : ?>
            <div class="score_marking_box b-none">
              <div class="no-flex_q_text <?= empty($answerQuestions["question_{$question}"]['menus']) ? 'd-flex-between' : '' ?>">
                <p class="q_text">
                  <span class="q_num"><?= "0$question" ?></span>
                  <span><?= $option['title'] ?></span>
                </p>
                <?php if (empty($answerQuestions["question_{$question}"]['menus'])) : ?>
                  <div class="score_input no-pic">
                      <span class="no-pic_score">0</span>
                      <span class="unit">点</span>
                      <input type="number" name="<?= "question_{$question}_score" ?>" value="0" hidden>
                  </div>
                <?php endif; ?>
              </div>
              <p class="q_text_comment"><?= !empty($option['sub_title_1']) ? $option['sub_title_1'] : '' ?></p>
              <p class="q_text_comment mb10"><?= !empty($option['sub_title_2']) ? $option['sub_title_2'] : '' ?></p>
              <?php if (!empty($answerQuestions["question_{$question}"]) && !empty($answerQuestions["question_{$question}"]['menus'])) : ?>
                <?php foreach ($answerQuestions["question_{$question}"]["menus"] as $menu => $answer) : ?>
                  <?php $menu = $menu + 1; ?>
                  <div class="wrap_menu">
                    <div class="score_marking_box">
                      <div class="flex_qa-btn-score">
                        <div class="qa_text">
                          <p class="q_text_sub"><?= "メニュー名" . $menu ?></p>
                          <p class="a_text mb0"><?= $answer['name'] ?></p>
                        </div>
                        <?php if (!empty($answer['image'])): ?>
                            <?php
                                $image = is_string($answer['image']) ? $answer['image'] : $answer['image']['path'];
                            ?>
                          <div class="btn_pic">
                            <a class="pic_block" id="pic_block_images_question_<?= "{$question}_{$menu}" ?>">
                              画像を見る
                              <?= $this->CustomHtml->imageUpload(h($image), ['id' => "pic_block_images_question_{$question}_{$menu}", 'hidden' => true]); ?>
                            </a>
                          </div>
                          <div class="score_input no-pic">
                            <span>
                                <?php $scoreQuestion = json_decode($restaurantQuestion->{"question_{$question}_menu_{$menu}_score"}, true);
                                    $score = (int) $scoreQuestion === 0 ? '' : $scoreQuestion['score'];
                                ?>
                                <?= $this->Form->text("question_{$question}_menu_{$menu}_score.score", ['default' => $score, 'type' => 'text', 'required' => false]) ?>
                            </span>
                            <span class="unit">点</span>
                          </div>
                        <?php else : ?>
                          <div class="btn_pic"></div>
                          <div class="score_input no-pic">
                            <span class="no-pic_score"><?= count($answer['ingredients']) >= 2 ? 2 : 0 ?></span>
                            <span class="unit">点</span>
                            <input type="number" name="<?= "question_{$question}_menu_{$menu}_score[score]" ?>"
                                value="<?= count($answer['ingredients']) >= 2 ? 2 : 0 ?>" hidden>
                          </div>
                        <?php endif; ?>
                      </div>
                      <?php if (!empty($errors) && !empty($errors["question_{$question}_menu_{$menu}_score"]["score"])): ?>
                        <div class="error-message text-right"><?= $errors["question_{$question}_menu_{$menu}_score"]["score"] ?></div>
                      <?php endif; ?>
                    </div>
                    <?php if($question === 1): ?>
                        <div class="check_inner_qa-score_block mb50">
                            <p class="check_inner_qa-score_title">地元産食材の使用について</p>
                            <div class="check_inner_a-score">
                            <p class="check_inner_a">地元産食材を2品以上使用しているか</p>
                            <?php
                                $localIngredients = array_filter($answer['ingredients'], fn($ingredient) => $ingredient['local_ingredient'] == true);
                                $bonusLocalIngredients = count($localIngredients) >= 2 ? 5 : 0;
                                if ($bonusLocalIngredients == 5) {
                                    $count++;
                                }
                            ?>
                            <?php if ($bonusLocalIngredients == 5): ?>
                                <p class="check_inner_score">
                                    <span class="check_inner_score_num">
                                        <?= $bonusLocalIngredients ?>
                                        <?php if (isset($count) && $count == 1): ?>
                                            <input type="number" name="<?= "question_{$question}_menu_{$menu}_score[local_ingredients_score]" ?>"
                                                value="<?= $bonusLocalIngredients ?>" hidden>
                                        <?php else: ?>
                                            <input type="number" name="<?= "question_{$question}_menu_{$menu}_score[local_ingredients_score]" ?>"
                                                value="0" hidden>
                                        <?php endif; ?>
                                    </span>
                                    <span class="check_inner_score_unit">点</span>
                                </p>
                            <?php else: ?>
                                <p class="check_inner_score">
                                    <span class="check_inner_score_num">
                                        0
                                        <input type="number" name="<?= "question_{$question}_menu_{$menu}_score[local_ingredients_score]" ?>"
                                            value="0" hidden>
                                    </span>
                                    <span class="check_inner_score_unit">点</span>
                                </p>
                            <?php endif; ?>
                            </div>
                        </div>
                    <?php endif; ?>
                    <?php foreach ($answer['ingredients'] as $index => $ingredient) : ?>
                      <!-- .wrap_inner-item -->
                      <div class="wrap_inner-item">
                        <!-- .check_info_box -->
                        <div class="check_info_box">
                          <?php if (!empty($ingredient['name'])): ?>
                            <div class="check_inner_qa-score_block check_info_inner">
                              <p class="check_inner_qa-score_title"><?= '食材その' . $index + 1 ?></p>
                              <div class="check_inner_a-score">
                                <p class="check_inner_a"><?= $ingredient['name'] ?></p>
                              </div>
                            </div>
                          <?php endif; ?>

                          <?php if (!empty($ingredient['supplier'])): ?>
                          <div class="check_inner_qa-score_block check_info_inner">
                            <p class="check_inner_qa-score_title"><?= '仕入れ先その' . $index + 1 ?></p>
                            <div class="check_inner_a-score">
                              <p class="check_inner_a"><?= $ingredient['supplier'] ?></p>
                            </div>
                          </div>
                          <?php endif; ?>

                          <?php if(isset($ingredient['local_ingredient'])): ?>
                          <div class="check_inner_qa-score_block check_info_inner">
                            <p class="check_inner_qa-score_title">食材は地元食材である。</p>
                            <div class="check_inner_a-score">
                              <p class="check_inner_a"><?= !empty($ingredient['local_ingredient']) ? 'はい' : 'いいえ' ?></p>
                            </div>
                          </div>
                          <?php endif; ?>
                        </div>
                        <!-- / .check_info_box -->
                        <?php foreach ($ingredient['extra_information'] as $type => $infos): ?>
                          <?php if (!empty($option['menus'][$index + 1]['ingredients'][1]['extra_information'][$type])): ?>
                            <?php $typeIngredient = $option['menus'][$index + 1]['ingredients'][1]['extra_information'][$type]; ?>
                            <div class="check_inner_qa-score_block <?= $type ?>">
                              <p class="check_inner_qa-score_title">
                                <?= $typeIngredient['title'] ?>
                              </p>
                              <?php foreach ($infos as $info): ?>
                                <div class="check_inner_a-score">
                                  <p class="check_inner_a"><?= $typeIngredient['options'][$info]['label'] ?></p>
                                  <p class="check_inner_score">
                                    <span class="check_inner_score_num"><?= $typeIngredient['options'][$info]['score'] ?></span>
                                    <input type="number" name="<?= "question_{$question}_menu_{$menu}_score[ingredients][$type][]" ?>"
                                        value="<?= $typeIngredient['options'][$info]['score'] ?>" hidden />
                                    <span class="check_inner_score_unit">点</span></p>
                                </div>
                              <?php endforeach; ?>
                            </div>
                          <?php endif; ?>
                        <?php endforeach; ?>
                        <?php if(!empty($ingredient['effort_image'])): ?>
                            <?php
                                $effortImage = is_string($ingredient['effort_image']) ? $ingredient['effort_image'] : $ingredient['effort_image']['path'];
                            ?>
                            <div class="score_marking_box">
                            <div class="flex_qa-btn-score only_btn">
                                <div class="btn_pic">
                                <a class="pic_block" id="pic_block_images_question_ingredient<?= "{$question}" ?>">
                                    画像を見る
                                    <?= $this->CustomHtml->imageUpload(h($effortImage), ['id' => "pic_block_images_question_ingredient_{$question}", 'hidden' => true]); ?>
                                </a>
                                </div>
                            </div>
                            </div>
                        <?php endif; ?>
                      </div>
                      <!-- / .wrap_inner-item -->
                    <?php endforeach; ?>
                  </div>

                <?php endforeach; ?>
              <?php endif; ?>
            </div>
          <?php elseif ($option['type'] == TYPE_CHECKBOX) : ?>
            <div class="wrap_qa_block">
              <div class="score_marking_box b-none">
                <div class="no-flex_q_text space_bottom <?= empty($answerQuestions["question_{$question}"]) ? 'd-flex-between' : '' ?>">
                  <p class="q_text">
                    <span class="q_num"><?= "0$question" ?></span>
                    <span><?= $option['title'] ?></span>
                  </p>
                  <?php if (empty($answerQuestions["question_{$question}"])) : ?>
                    <div class="score_input no-pic">
                      <span class="no-pic_score">0</span>
                      <span class="unit">点</span>
                      <input type="number" name="<?= "question_{$question}_score" ?>" value="0" hidden>
                    </div>
                  <?php endif; ?>
                </div>
                <?php if (!empty($answerQuestions["question_{$question}"]) && !empty($answerQuestions["question_{$question}"]['answers'])) : ?>
                  <?php foreach ($answerQuestions["question_{$question}"]['answers'] as $key => $answer) : ?>
                    <div class="score_marking_box">
                      <div class="flex_qa-btn-score">
                        <div class="qa_text">
                          <p class="q_text_sub"></p>
                          <p class="a_text"><?= $option['options'][$answer]['label'] ?></p>
                        </div>
                        <div class="btn_pic">
                        </div>
                        <div class="score_input no-pic">
                          <span class="no-pic_score"><?= $option['options'][$answer]['score'] ?></span>
                          <input type="number" name="<?= "question_{$question}_score[$answer]" ?>" value="<?= $option['options'][$answer]['score'] ?>" hidden>
                          <span class="unit">点</span>
                        </div>
                      </div>
                    </div>
                  <?php endforeach; ?>
                <?php endif; ?>
              </div>
              <?php if (!empty($answerQuestions["question_{$question}"]['effort_detail'])) : ?>
                <div class="free_text_block">
                  <p class="free_text_title">取組内容</p>
                  <p class="free_text_detail"><?= $answerQuestions["question_{$question}"]['effort_detail'] ?></p>
                </div>
              <?php endif; ?>
              <?php if (!empty($answerQuestions["question_{$question}"]['effort_image'])) : ?>
                <?php
                    $effortImageQuestion = is_string($answerQuestions["question_{$question}"]['effort_image'])
                        ? $answerQuestions["question_{$question}"]['effort_image']
                        : $answerQuestions["question_{$question}"]['effort_image']['path'];
                ?>
                <div class="score_marking_box">
                  <div class="flex_qa-btn-score only_btn">
                    <div class="btn_pic">
                      <a class="pic_block" id="pic_block_images_question_<?= "{$question}" ?>">
                        画像を見る
                        <?= $this->CustomHtml->imageUpload(h($effortImageQuestion), ['id' => "pic_block_images_question_{$question}", 'hidden' => true]); ?>
                      </a>
                    </div>
                  </div>
                </div>
              <?php endif; ?>
            </div>
          <?php endif; ?>
        <?php endforeach; ?>
      </div>
      <!-- / #tab_03_content -->
    </div>
  </section>

  <section class="btn_sec">
    <?= $this->Form->submit('合計点のページに進む') ?>
    <p><button class="backBtn">
        <?= $this->Html->link('回答一覧に戻る', $this->Url->build([
          'prefix' => 'Checker',
          'controller' => 'RestaurantApplication',
          'action' => 'list',
        ], ['fullBase' => true])) ?>
      </button></p>
  </section>
  <?= $this->Form->end(); ?>
</article>
