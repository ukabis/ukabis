<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '生産者 申請完了ページ');?>
<?php $this->assign('farm_application', $this->Html->script('farm_application'));?>

<section class="content">
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
                        <p class="qa_title">申請完了</p>
                    </dd>
                </dl>
            </div>
            <div class="contents">

                <div class="complete_text">
                    <p>申請されました。</p>
                    <p>結果は、<br>ふじのくにSDGs認証 申請システムより<br>メールが送付されますので<br>少々お待ちください</p>
                    <p>ご利用ありがとうございました</p>
                </div>


                <div class="back-mypage_btn_block">
                    <p>
                        <button>
                            <?= $this->Html->link('マイページへ戻る', $this->Url->build([
                                'prefix' => 'Farms',
                                'controller' => 'Farm',
                                'action' => 'index'
                            ], ['fullBase' => true])) ?>
                        </button>
                    </p>
                </div>
            </div>

        </section>

    </article>
</section>

