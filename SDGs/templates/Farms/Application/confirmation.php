<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '生産者 入力確認ページ');?>
<?php $this->assign('farm_application', $this->Html->script('farm_application'));?>

<section class="content confirmation_page">
    <article class="wrap_contents">
        <section class="name_sec">
            <?php
                if ($user->office && $user->office->business) {
                    echo "<h2>" . h($user->office->business->representative_name) . "</h2>";
                }
            ?>
        </section>

        <section class="contents_sec">
            <div class="contents_title">
                <dl class="question q_title">
                    <!-- <dt class="q_no"></dt> -->
                    <dd class="q_text">
                        <p class="qa_title">入力確認</p>
                    </dd>
                </dl>
            </div>
            <div class="contents">
                <div class="wrap_disp">
                <?php foreach ($farmQuestions as $key => $question) : ?>
                    <div class="disp_box" data-question="question_<?php echo $key; ?>">
                        <dl class="disp_title">
                            <dt class="disp_number">
                                <span><?php echo $key; ?></span>
                                <?php if($question->group_question): ?>
                                    <span>(<?= h($question->group_question) ?>)</span>
                                <?php endif ?>
                            </dt>
                            <dd>
                                <h3><?php echo h($question->title); ?></h3>
                            </dd>
                        </dl>
                        <dl>
                            <dt></dt>
                            <dd class="disp_answer">
                                <p class="text_red">未入力</p>
                            </dd>
                        </dl>
                        <p class="disp_again"><a href="/farms/application/question/<?php echo $key; ?>?mode=process">確認する</a></p>
                    </div>
                <?php endforeach; ?>
                </div>
                <div class="registration_btn_block">
                    <p><button id="btn_apply" class="btn-disabled"><a href="javascript:void(0);">申請する</a></button></p>
                </div>

                <div class="comment_before">
                    <p>本申請に際し、一切の虚偽はありません。</p>
                </div>

                <div class="prev_btn_block">
                    <p><button><a href="/farms/application/question/<?php echo $total; ?>?mode=process">戻る</a></button></p>
                </div>
            </div>
        </section>
    </article>
</section>
