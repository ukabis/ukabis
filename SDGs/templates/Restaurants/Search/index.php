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
<?php $this->assign('title', '飲食店／生産者 申請済み 検索ページ');?>
<section class="content">
    <article class="wrap_contents">
        <section class="contents_sec registered">
            <div class="contents_title_img">
                <h2 class="tit_img">
                    <?= $this->Html->image('tit_goals.png', ['alt' => 'SUSTAINABLE DEVELOPMENT GOALS']) ?>
                </h2>
            </div>
            <div class="contents registered_farm_contents">
                <div class="wrap_registered_search">
                    <?= $this->Form->create(null, [
                        'url' => [
                            'prefix' => 'Restaurants',
                            'controller' => 'Search',
                            'action' => 'list'
                        ],
                        'type' => 'get'
                    ]); ?>
                    <h3>認定されている生産者の一覧検索</h3>
                    <div class="edit_input">
                        <?= $this->Form->control('area', [
                            'placeholder' => 'エリア',
                            'label' => false,
                            'required' => false,
                            'maxlength' => 20,
                            'default' => $this->request->getQuery('area')
                        ]) ?>
                        <?= $this->Form->control('keyword', [
                            'placeholder' => 'キーワード',
                            'label' => false,
                            'required' => false,
                            'maxlength' => 20,
                            'default' => $this->request->getQuery('amp;keyword')
                        ]) ?>
                    </div>
                    <div class="btn_block farm_page">
                        <p>
                            <?= $this->Form->button('生産者検索', ['class' => 'btn_block farm_page search_farm']); ?>
                        </p>
                    </div>
                <?= $this->Form->end(); ?>
                </div>
            </div>
        </section>
    </article>
</section>
