<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '生産者 質問ページ');?>
<?php $this->assign('farm_application', $this->Html->script('farm_application'));?>

<section class="content">
    <article class="wrap_contents">
        <section class="name_sec">
            <?php
                if ($user->office && $user->office->business) {
                    echo "<h2>" . h($user->office->business->representative_name) . "</h2>";
                }
            ?>
            <p class="howmany">
                <?php echo sprintf("（%d問目／%d問中）", $questionNo, $total); ?>
            </p>
        </section>

        <section class="contents_sec">
            <?php echo $this->Form->hidden('key', ['value' => sprintf("question_%d", $questionNo)]); ?>
            <div class="contents_title">
                <dl class="question">
                    <dt class="q_no">
                        <span><?= $questionNo ?></span>
                        <?php if($farmQuestion->group_question): ?>
                            <span>(<?= h($farmQuestion->group_question) ?>)</span>
                        <?php endif ?>
                    </dt>
                    <dd class="q_text">
                        <p>
                            <?= h($farmQuestion->title) ?>
                        </p>
                    </dd>
                </dl>
            </div>
            <div class="contents question-content">
                <form id="form1" enctype="multipart/form-data">
                    <?php foreach($farmQuestion->options as $key => $option): ?>
                        <div class="answer_block answer-box">
                            <?php if($farmQuestion->allow_multi): ?>
                                <label class="wrap_label">
                                    <?php echo $this->Form->checkbox("question{$questionNo}_{$key}", ['value' => $key, 'hiddenField' => false]); ?>
                                    <p class="cb_text"><?= h($option->title) ?></p>
                                </label>
                            <?php else: ?>
                                <button id="btn_0<?= $key + 1 ?>" class="answer_box question<?= $questionNo ?>" type="button">
                                    <label><input type="radio" name="question<?= $questionNo ?>" value="<?= $key ?>"><?= h($option->title) ?></label>
                                </button>
                            <?php endif ?>
                            <?php if($option->has_file): ?>
                                <p class="answer-btn-upload">
                                    <label for="<?= "question{$questionNo}_{$key}" ?>">
                                        <?= $option->upload_file_label ?? $farmQuestion->upload_file_label ?? '認定証添付' ?>
                                    </label>
                                    <?php echo $this->Form->file('', ['id' => "question{$questionNo}_{$key}", 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                                </p>
                            <?php endif ?>
                        </div>
                    <?php endforeach ?>

                    <?php if($farmQuestion->has_file): ?>
                        <div class="answer-box upload-file">
                            <p class="answer-btn-upload">
                                <label for="<?= "question{$questionNo}" ?>">
                                    <?= $option->upload_file_label ?? $farmQuestion->upload_file_label ?? '認定証添付' ?>
                                </label>
                                <?php echo $this->Form->file("question{$questionNo}", ['id' => "question{$questionNo}", 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                            </p>
                        </div>
                    <?php endif ?>

                </form>
                <div class="next_btn_block">
                    <?php if ($questionNo === $total) : ?>
                        <p>
                            <button>
                                <?php
                                    echo $this->Html->link('次へ', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'Application',
                                        'action' => 'confirmation',
                                        '?' => ['mode' => 'process']
                                    ], ['fullBase' => false]),
                                        ['class' => 'btn_to_confirm']);
                                ?>
                            </button>
                        </p>
                    <?php else: ?>
                        <p>
                            <button>
                                <?php
                                echo $this->Html->link('次へ', $this->Url->build([
                                    'prefix' => 'Farms',
                                    'controller' => 'Application',
                                    'action' => 'index',
                                    sprintf("%d?mode=process", $questionNo + 1)
                                ], ['fullBase' => false]),
                                    ['class' => 'next_question']);
                                ?>
                            </button>
                        </p>
                    <?php endif; ?>
                </div>
                <div class="prev_btn_block">
                    <p>
                        <button>
                            <?php switch ($questionNo) :
                                case 1:
                                    echo $this->Html->link('戻る', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'Farm',
                                        'action' => 'index',
                                    ], ['fullBase' => false]));
                                    break;

                                default:
                                    echo $this->Html->link('戻る', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'Application',
                                        'action' => 'index',
                                        sprintf("%d?mode=process", $questionNo - 1)
                                    ], ['fullBase' => false]),
                                        ['class' => 'prev_question']);
                                    break;
                            ?>
                            <?php endswitch; ?>
                        </button>
                    </p>
                </div>
                <p class="again_return">
                    <?php
                        echo $this->Html->link('入力確認へ', $this->Url->build([
                            'prefix' => 'Farms',
                            'controller' => 'Application',
                            'action' => 'confirmation',
                            '?' => ['mode' => 'process']
                        ], ['fullBase' => false]),
                        ['class' => 'btn_to_confirm']);
                    ?>
                </p>
            </div>
        </section>
    </article>
</section>

