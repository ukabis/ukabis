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

?>
<?php $this->assign('title', '生産者 設問・画像 合計点ページ');?>
<article class="contents">
  <section class="title_sec">
    <h2>生産者 設問・画像 合計点ページ</h2>
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
      <tr style="width: 100%">
        <td><?= $farmAnswer->id ?></td>
        <td><?= $this->CustomHtml->renderTextWithMaxLength($farmAnswer->office->business->representative_name) ?></td>
        <td><?= $this->CustomHtml->renderTextWithMaxLength($farmAnswer->office->name) ?></td>
        <td><?= $farmAnswer->modified_at->format('d/m/y h:i A') ?></td>
        <td class="no-wrap"><?= $farmAnswer->status_label ?></td>
      </tr>
    </table>
  </section>
  <?= $this->Form->create($farmAnswer) ?>
    <section class="score_total_sec">
      <p><span class="total_score_title">合計点数</span><span class="total_score"><?= $total ?? $farmAnswer->total_score ?? 0 ?></span><span class="total_score_unit">点</span></p>
    </section>

    <section class="score_marking_sec">
      <div class="tab_sec">
        <input id="tab_01" type="radio" name="tab_item">
        <label for="tab_01" class="tab_item first-child">登録店舗</label>
        <input id="tab_02" type="radio" name="tab_item" checked>
        <label for="tab_02" class="tab_item">設問／採点</label>
        <!-- #登録店舗 tab -->
        <div class="tab_content" id="tab_01_content">
          <table class="row_table">
            <tr>
              <th>業種</th>
              <td><?= $farmAnswer->office->offices_introduction ?></td>
            </tr>
            <tr>
              <th>企業名</th>
              <td><?= h($farmAnswer->office->business->name)  ?></td>
            </tr>
            <tr>
              <th>企業名（カナ）</th>
              <td><?= h($farmAnswer->office->business->name_kana) ?></td>
            </tr>
            <tr>
              <th>代表取締役</th>
              <td><?= h($farmAnswer->office->business->representative_name) ?></td>
            </tr>
            <tr>
              <th>郵便番号</th>
              <td><?= h($farmAnswer->office->zip_code) ?></td>
            </tr>
            <tr>
              <th>住所</th>
              <td><?= h("{$farmAnswer->office->prefecture} {$farmAnswer->office->city} {$farmAnswer->office->street}") ?></td>
            </tr>
            <tr>
              <th>電話番号</th>
              <td><?= $farmAnswer->office->tel ?></td>
            </tr>
            <tr>
              <th>FAX番号</th>
              <td><?= $farmAnswer->office->fax ?></td>
            </tr>
            <tr>
              <th>店舗名</th>
              <td><?= h($farmAnswer->office->name) ?></td>
            </tr>
            <tr>
              <th>店舗名（カナ）</th>
              <td><?= h($farmAnswer->office->name_kana) ?></td>
            </tr>
          </table>
        </div>
        <!-- / #end tab -->

        <!-- #設問／採点 tab -->
        <div class="tab_content" id="tab_02_content">
            <?php
              $answerQuestions = json_decode($farmAnswer->answer_questions, true)
            ?>
            <?php foreach ($farmQuestions as $key => $question) : ?>
              <div class="score_marking_box flex-score qa-score">
                <div class="no-flex_q_text">
                  <div class="ttl_text">
                    <p class="q_text">
                      <span><?= str_pad($key, 2, '0', STR_PAD_LEFT) ?></span>
                      <?php if($question['group_question']): ?>
                        <span><?= "({$question['group_question']})" ?></span>
                      <?php endif ?>
                      <br>
                      <span><?= h($question['title']) ?></span>
                    </p>
                  </div>
                </div>
                <div class="sub-qa">
                  <?php if(isset($answerQuestions["question_{$key}"])): ?>
                    <?php foreach($answerQuestions["question_{$key}"] as $index => $answer): ?>
                      <div class="opt-qa">
                        <div class="opt-qa_text">
                          <p class="q_text_sub"></p>
                          <p class="a_text_sub">
                            <?= $question['options'][$answer['answer']]['title'] ?>
                          </p>
                        </div>
                        <div class="opt-qa_action">
                          <div class="score_input no-pic">
                            <div class="disp_score">
                              <?php
                                $questionName = "question{$key}_score";
                                $score = 0;
                                if (array_key_exists($answer['answer'], json_decode($farmAnswer->$questionName, true))) {
                                  $score = json_decode($farmAnswer->$questionName, true)[explode('_', $answer['question'])[1] ?? $answer['answer']] ?? '0';
                                }
                              ?>
                              <p><?= $score ?>点</p>
                            </div>
                          </div>
                        </div>
                      </div>
                    <?php endforeach ?>
                  <?php else: ?>
                    <div class="opt-qa">
                      <div class="opt-qa_text">
                        <p class="a_text_sub">未入力</p>
                      </div>
                      <div class="opt-qa_action">
                        <div class="score_input no-pic">
                          <div class="disp_score">
                            <p>0点</p>
                          </div>
                        </div>
                      </div>
                    </div>
                  <?php endif ?>
                </div>
              </div>
            <?php endforeach ?>
          </div>
      </div>
    </section>

    <section class="btn_sec">
      <p>
        <?= $this->Form->button('生産者 メール送信ページへ', ['type' => 'submit']); ?>
      </p>
      <p>
        <button class="backBtn" type="button">
          <?= $this->Html->link('生産者・画像設問採点ページへ戻る', $this->Url->build([
            'prefix' => 'Checker',
            'controller' => 'FarmApplication',
            'action' => 'score',
            'id' => $farmAnswer->id
          ], ['fullBase' => true])) ?>
        </button>
      </p>
    </section>
  <?= $this->Form->end() ?>
</article>
