<?php
/**
 * CakePHP(tm) : Rapid Development Framework (https://cakephp.org)
 * Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 *
 * Licensed under The MIT License
 * For full copyright and license information, please see the LICENSE.txt
 * Redistributions of files must retain the above copyright notice.
 *
 * @copyright     Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 * @link          https://cakephp.org CakePHP(tm) Project
 * @since         0.10.0
 * @license       https://opensource.org/licenses/mit-license.php MIT License
 * @var \App\View\AppView $this
 */

?>
<?php $this->assign('title', '生産者 出荷 登録ページ') ?>
<article class="wrap_contents farm_request_registration">
    <section class="name_sec request_edit">
        <h2>いまどき！出荷登録</h2>
        <div class="contents contents_request">
            <?= $this->Form->create($request); ?>
                <div class="edit_input">
                    <?php
                        echo $this->Form->control('food', ['placeholder' => 'フード', 'label' => false, 'required' => false, 'maxlength' => 20]);
                        echo $this->Form->control('comment', ['placeholder' => 'コメント入力（20文字）', 'label' => false, 'required' => false, 'maxlength' => 20]);
                    ?>
                  	<?= $this->Form->button('出荷登録', ['class' => 'btn_color_orange']); ?>
                </div>
            <?= $this->Form->end(); ?>
        </div>
    </section>

    <section class="contents_sec registered pt0pb0 edit">
        <div class="contents_edit_title">
            <h3>過去のリクエスト一覧</h3>
        </div>

        <div class="contents mb0">
            <div class="edit_input">
                <?php if(count($listRequest)): ?>
                    <?php foreach ($listRequest as $request) : ?>
                    <p class="request_edit_input">
                        <input type="text" class="request_product disabled" readonly placeholder="<?= $request->food ?>">
                        <input type="text" class="request_day disabled" readonly placeholder="<?= $request->created_at->format('Y/m/d') ?>">
                    </p>
                    <p><input type="text" class="request_comment disabled" readonly placeholder="<?= $request->comment ?>"></p>
                    <?php endforeach; ?>
                <?php else: ?>
                    <p class="text-center">まだ要望がない。</p>
                <?php endif; ?>
                <p>
                    <button class="btn_color_orange">
                        <?= $this->Html->link('飲食店を探す', $this->Url->build([
                            'prefix' => 'Farms',
                            'controller' => 'RestaurantRequest',
                            'action' => 'search'
                        ], ['fullBase' => true])) ?>
                    </button>
                </p>
            </div>
        </div>
    </section>
</article>
