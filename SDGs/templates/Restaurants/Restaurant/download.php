<?php

/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '飲食店 認定マーク、認定証ダウンロード');?>
<section class="content mypage_download">
    <article class="wrap_contents pb1rem">
        <section class="name_sec registered_edit">
            <h2>富士ファーム</h2>
        </section>

        <section class="contents_sec registered pt0pb0 edit">
            <div class="contents_edit_title">
                <h3>認定マーク、認定証ダウンロード</h3>
            </div>

            <div class="contents">
                <div class="edit_contents">
                    <p>認証：<?= $user->office->certificate['rank_label'] ?></p>
                    <p>申請者：<?= $user->office->business->representative_name ?></p>
                </div>

                <div class="edit_btn">
                    <p>
                        <button>
                            <?= $this->Form->postLink('認定マークダウンロード',
                                $this->Url->build([
                                    'prefix' => 'Restaurants',
                                    'controller' => 'Download',
                                    'action' => 'downloadCertificate'
                                ], ['fullBase' => true]),
                                [
                                    'method'  => 'POST',
                                    'data'    => [
                                        'target_file' => FILE_MARK
                                    ]
                                ])
                            ?>
                        </button>
                    </p>
                    <p>
                        <button>
                            <?= $this->Form->postLink('認定証ダウンロード',
                                $this->Url->build([
                                    'prefix' => 'Restaurants',
                                    'controller' => 'Download',
                                    'action' => 'downloadCertificate'
                                ], ['fullBase' => true]),
                                [
                                    'method'  => 'POST',
                                    'data'    => [
                                        'target_file' => FILE_CERTIFICATE
                                    ]
                                ])
                            ?>
                        </button>
                    </p>
                </div>
            </div>

            <div class="btn_edit_block">
                <p class="edit_back_btn">
                    <button>
                        <?= $this->Html->link('戻る', $this->Url->build([
                            'prefix' => 'Restaurants',
                            'controller' => 'Restaurant',
                            'action' => 'edit'
                        ], ['fullBase' => true])) ?>
                    </button>
                </p>
                <p class="edit_save_btn">
                    <button>
                        <?= $this->Html->link('保存する', $this->Url->build([
                            'prefix' => 'Restaurants',
                            'controller' => 'Restaurant',
                            'action' => 'index',
                        ], ['fullBase' => true])) ?>
                    </button>
                </p>
            </div>
        </section>
    </article>
</section>
