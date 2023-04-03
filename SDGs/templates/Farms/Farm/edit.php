<?php

/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '生産者マイページ');?>
<section class="content farm_mypage farm_mypage_edit">
    <div class="wrap_page">
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
                    <h3>マイページ</h3>
                </div>
                <div class="contents">
                    <div class="btn_block">
                        <div class="btn_top">
                            <p>
                                <button>
                                    <?= $this->Html->link('企業/店舗情報編集', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'President',
                                        'action' => 'edit',
                                    ], ['fullBase' => true])) ?>
                                </button>
                            </p>
                            <p class="caution">※企業/店舗情報が未登録の場合、SDGs申請が完了しませんので必ず登録をしてください。</p>
                            <p>
                                <?php if (in_array($user->office->offices_certified_rank, APPROVE_RANK_LIST)) : ?>
                                    <button>
                                        <?= $this->Html->link('認定マークダウンロード', $this->Url->build([
                                            'prefix' => 'Farms',
                                            'controller' => 'Farm',
                                            'action' => 'download',
                                        ], ['fullBase' => true])) ?>
                                    </button>
                                <?php else : ?>
                                    <button class="btn-disabled"><a href="javascript:void(0);">認定マークダウンロード</a></button>
                                <?php endif ; ?>
                            </p>
                            <p>
                                <button class="btn_color_orange">
                                    <?= $this->Html->link('登録情報を見る', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'Farm',
                                        'action' => 'detail',
                                    ], ['fullBase' => true])) ?>
                                </button>
                            </p>
                        </div>
                        <div class="btn_edit_block">
                            <p class="edit_back_btn">
                                <button>
                                    <?= $this->Html->link('戻る', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'Farm',
                                        'action' => 'index',
                                    ], ['fullBase' => true])) ?>
                                </button>
                            </p>
                        </div>
                    </div>
                </div>

            </section>
        </article>
    </div>
</section>
