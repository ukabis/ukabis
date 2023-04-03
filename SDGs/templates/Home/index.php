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

$this->disableAutoLayout();
?>
<!DOCTYPE html>
<html lang="ja">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <?= $this->Html->css(['style']) ?>
    <title>検索 トップページ</title>
</head>
<body class="common_page">
    <div class="wrap_page">
        <div class="search_top_title">
            <h1>ふじのくにSDGs認証<br>申請システム</h1>
        </div>

        <article class="search_main_area">
            <section class="search_contents_sec search_store">
                <div class="wrap_search_h2">
                    <h2>認定されている飲食店の一覧検索</h2>
                </div>
                <div class="wrap_search_block">
                  <?= $this->Form->create(null, [
                        'url' => [
                            'controller' => 'SearchRestaurant',
                            'action' => 'list'
                        ],
                        'type' => 'get'
                    ]); ?>
                    <div class="edit_input store">
                      <?= $this->Form->control('restaurant.area', [
                            'placeholder' => 'エリア',
                            'label' => false,
                            'required' => false,
                            'maxlength' => 20,
                            'default' => $this->request->getQuery('restaurant.area')
                        ]) ?>
                        <?= $this->Form->control('restaurant.keyword', [
                            'placeholder' => 'キーワード',
                            'label' => false,
                            'required' => false,
                            'maxlength' => 20,
                            'default' => $this->request->getQuery('restaurant.keyword')
                        ]) ?>
                    </div>

                    <div class="btn_block store_page">
						<p>
							<?= $this->Form->button('飲食店検索', ['class' => 'btn_block farm_page search_farm']); ?>
						</p>
                    </div>
                <?= $this->Form->end(); ?>
                </div>
            </section>
        </article>

        <article class="search_main_area">
            <section class="search_contents_sec search_farm">
                <div class="wrap_search_h2">
                    <h2>認定されている生産者の一覧検索</h2>
                </div>
                <div class="wrap_search_block">
                  <?= $this->Form->create(null, [
                        'url' => [
                            'controller' => 'SearchFarm',
                            'action' => 'list'
                        ],
                        'type' => 'get'
                    ]); ?>
                    <div class="edit_input farm">
                      <?= $this->Form->control('farm.area', [
                            'placeholder' => 'エリア',
                            'label' => false,
                            'required' => false,
                            'maxlength' => 20,
                            'default' => $this->request->getQuery('farm.area')
                        ]) ?>
                        <?= $this->Form->control('farm.keyword', [
                            'placeholder' => 'キーワード',
                            'label' => false,
                            'required' => false,
                            'maxlength' => 20,
                            'default' => $this->request->getQuery('farm.keyword')
                        ]) ?>
                    </div>

                    <div class="btn_block farm_page">
						<p>
							<?= $this->Form->button('生産者検索', ['class' => 'btn_block farm_page search_farm']); ?>
						</p>
                    </div>
                <?= $this->Form->end(); ?>
                </div>
            </section>
        </article>

        <footer>
            <p><?= $this->Html->image('logo.png', ['alt' => 'powered by ukabis']) ?></p>
            <p class="copy">powered by ukabis</p>
        </footer>
    </div>
</body>
</html>
