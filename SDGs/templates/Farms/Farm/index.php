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
<?php $this->assign('title', '生産者マイページ');?>
<section class="content">
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
                                    <button>
                                        <?= $this->Html->link('SDGs申請', $this->Url->build([
                                            'prefix' => 'Farms',
                                            'controller' => 'Application',
                                            'action' => 'index',
                                            '1',
                                        ], ['fullBase' => true])) ?>
                                    </button>
                                </button>
                            </p>
                            <p><button class="btn_color_orange">
                                <?= $this->Html->link('いまどき！出荷登録', $this->Url->build([
                                    'prefix' => 'Farms',
                                    'controller' => 'Request',
                                    'action' => 'register'
                                ], ['fullBase' => true])) ?></button></p>
                            <p>
                                <button class="btn_color_orange">
                                    <?= $this->Html->link('飲食店一覧', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'Search',
                                        'action' => 'index'
                                    ], ['fullBase' => true])) ?>
                                </button>
                            </p>
                            <p>
                            <button class="btn_mypage_edit btn_color_orange">
                                <?= $this->Html->link('登録情報編集', $this->Url->build([
                                        'prefix' => 'Farms',
                                        'controller' => 'Farm',
                                        'action' => 'edit'
                                    ], ['fullBase' => true])) ?>
                            </button>
                        </p>
                            <p class="caution">※企業/店舗情報が未登録の場合、SDGs申請が完了しませんので必ず登録をしてください。</p>
                        </div>
                    </div>
                </div>
            </section>
        </article>
    </div>
</section>
