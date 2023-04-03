<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '飲食店 質問ページ');?>
<?php $this->assign('restaurant_application_script', $this->Html->script('restaurant_application'));?>
<?php $this->assign('component_manager_script', $this->Html->script('component_manager'));?>

<section class="content">
    <article class="wrap_contents">
        <section class="name_sec">
            <h2>申請情報</h2>
            <p class="howmany">
                <?= sprintf("（%d問目／%d問中）", $questionNo, $total); ?>
            </p>
        </section>

        <section class="contents_sec">
            <?= $this->Form->hidden('key', ['value' => sprintf("question_%d", $questionNo)]); ?>
            <?= $this->Form->hidden('type', ['value' => $restaurantQuestion->type]); ?>
            <?= $this->Form->hidden('menuComp', ['value' => $menuComp ]); ?>

            <div class="contents_title">
                <dl class="question">
                    <dt class="q_no"><?= str_pad(h($questionNo), 2, '0', STR_PAD_LEFT) ?></dt>
                    <dd class="q_text">
                        <p><?= h($restaurantQuestion->title) ?></p>
                    </dd>
                </dl>

                <div class="wrap_comment">
                    <?php if (isset($restaurantQuestion->sub_title_1)) : ?>
                        <p class="comment"><?= h($restaurantQuestion->sub_title_1)  ?></p>
                    <?php endif; ?>

                    <?php if (isset($restaurantQuestion->sub_title_2)) : ?>
                        <p class="comment"><?= h($restaurantQuestion->sub_title_2)  ?></p>
                    <?php endif; ?>
                </div>
            </div>
            <div class="contents">
                <?php if ($restaurantQuestion->type === TYPE_MULTI_MENU) : ?>
                    <div class="answer_title">
                        <h3><?= h($restaurantQuestion->answer_title) ?></h3>
                    </div>

                    <div class="menu_sec">
                    <?php foreach ($restaurantQuestion->menus as $menuKey => $item): ?>
                    <form class="menu_form" id="menu_form_<?= $menuKey ?>" enctype="multipart/form-data">
                        <?= $this->Form->hidden('menu_key', ['value' => $menuKey ]); ?>
                        <div class="wrap_answer_block">
                            <div class="answer_title_sub">
                                <p>メニュー名<?= $menuKey ?></p>
                            </div>
                            <div class="wrap_answer_form-btn_box">
                                <div class="answer_form-btn_box_change">
                                    <p class="answer_form"><input type="text" maxlength="200" name="menu_name" placeholder="メニュー名<?= $menuKey ?>"></p>
                                </div>
                                <div class="img_btn_block">
                                    <p><img id="menu_image_src_<?= $menuKey ?>" src="/images/img_no-image.png" alt="メニュー<?= $menuKey ?>"></p>
                                    <p class="answer_btn">
                                        <label for="menu_image_<?= $menuKey ?>">
                                            <i class="fas fa-paperclip"></i>
                                        </label>
                                        <?= $this->Form->file('answer_file', ['id' => 'menu_image_' . $menuKey, 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                                        <?= $this->Form->hidden('menu_image_path', ['data-image-id' => 'menu_image_' . $menuKey, 'value' => '']); ?>
                                    </p>
                                </div>

                                <?php if (property_exists($item, 'public_period_start') && property_exists($item, 'public_period_end')) : ?>
                                    <div class="calendar_head">
                                        <div class="calendar_message"><span></span></div>
                                        <div class="wrap_calendar_title">
                                            <p>掲載期間（開始）</p>
                                            <p>掲載期間（終了）</p>
                                        </div>
                                    </div>
                                    <div class="inner_form wrap_calendar">
                                        <p class="answer_form calendar start"><input type="date" class="public_period" name="public_period_start" placeholder="掲載期間（1980.1 〜）"></p>
                                        <p class="wave">～</p>
                                        <p class="answer_form calendar end"><input type="date" class="public_period" name="public_period_end" placeholder="掲載期間（1980.1 〜）"></p>
                                    </div>
                                <?php endif; ?>

                                <div class="ingredient_contents">
                                    <?php foreach ($item->ingredients as $ingKey => $ingredient) : ?>
                                    <div class="ingredient_sec" id="menu_<?= $menuKey?>_ingredient_<?= mt_rand(100, 999999) ?>"_>
                                        <div class="inner_form">
                                            <p class="answer_form"><input type="text" maxlength="200" name="ingredient_name" placeholder="食材その<?= $ingKey ?>"></p>

                                            <?php if (property_exists($ingredient, 'supplier')) : ?>
                                                <p class="answer_form"><input type="text" maxlength="200" name="ingredient_supplier" placeholder="仕入れ先名その<?= $ingKey ?>"></p>
                                            <?php endif; ?>
                                        </div>

                                        <?php if (property_exists($ingredient, 'local_ingredient')) : ?>
                                            <div class="inner_box">
                                                <div class="input_group">
                                                    <p>
                                                        <label>
                                                            <input type="checkbox" name="local_ingredient">
                                                            <span class="inner_text">食材は地元食材である。</span>
                                                        </label>
                                                    </p>
                                                </div>
                                            </div>
                                        <?php endif; ?>

                                        <?php foreach ($ingredient->extra_information as $infoKey => $infoItem) : ?>
                                        <?php $randomExtraId = mt_rand(100, 999999);?>
                                        <div class="extra_info_sec" id="menu_<?= $menuKey ?>_extra_info_<?= $randomExtraId ?>">
                                            <div class="inner_box">
                                                <p class="inner_title"><?= h($infoItem->title) ?></p>
                                                <div class="input_group">
                                                    <?php foreach ($infoItem->options as $optKey => $option) : ?>
                                                        <p>
                                                            <label>
                                                                <input type="<?= h($infoItem->type) ?>"
                                                                       name="<?= h($infoItem->name) ."_". $randomExtraId ?>"
                                                                       data-name="<?= h($infoItem->name) ?>"
                                                                       value="<?= $optKey ?>">
                                                                <span class="inner_text"><?= $option->label ?></span>
                                                            </label>
                                                        </p>
                                                    <?php endforeach; ?>
                                                </div>
                                            </div>
                                        </div>
                                        <?php endforeach; ?>

                                        <?php if (property_exists($ingredient,'effort_image')) : ?>
                                        <div class="inner_box other_box">
                                            <div class="wrap_menu_img_btn">
                                                <button type="button" class="menu_img_store">
                                                    <label for="<?= "menu_{$menuKey}_effort_image_ingredient_{$ingKey}" ?>">
                                                        取り組みが確認できる写真添付
                                                        <span class="file_name"></span>
                                                    </label>
                                                    <?= $this->Form->file('answer_file', ['id' => 'menu_' .$menuKey. '_effort_image_ingredient_' . $ingKey, 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                                                    <?= $this->Form->hidden('effort_image', ['data-image-id' => 'menu_' .$menuKey. '_effort_image_ingredient_' . $ingKey, 'value' => '']); ?>
                                                </button>
                                            </div>
                                        </div>
                                        <?php endif; ?>

                                        <div class="comment_plus-btn">
                                            <p>食材を追加する場合は、<br>こちらの「＋」ボタンをタップしてください。<br>
                                                <span class="in_comment">※食材は、1メニューにつき、2つまで。</span></p>
                                        </div>

                                        <div class="inner_btn_box">
                                            <div class="wrap_inner_btn">
                                                <button type="button" class="inner_btn" onclick="addIngredient(this, <?= $menuKey ?>);">
                                                    <p class="symbol">＋</p>
                                                </button>
                                                <p class="btn_text">食材を追加</p>
                                            </div>
                                            <div class="wrap_inner_btn">
                                                <button type="button" class="inner_btn" onclick="removeIngredient(this);">
                                                    <p class="symbol">－</p>
                                                </button>
                                                <p class="btn_text">食材を削除</p>
                                            </div>
                                        </div>
                                    </div>
                                    <?php endforeach; ?>
                                </div>

                                <div class="next_end_btn_box">
                                    <div>
                                        <button type="button" class="next_btn" onclick="addMenu();">
                                            <p class="btn_text">別のメニューを登録</p>
                                        </button>
                                    </div>
                                    <div>
                                        <button type="button" class="end_btn" onclick="removeMenu(this);">
                                            <p class="btn_text">メニューを削除</p>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </form>
                    <?php endforeach; ?>
                    </div>
                    <?php if (property_exists($restaurantQuestion,'menu_table_image') && property_exists($restaurantQuestion,'store_image')) : ?>
                        <div class="inner_box">
                            <p class="inner_btn_title">登録したメニューのメニュー表および店内で提示している画像を登録してください。</p>
                            <div class="wrap_menu_img_btn">
                                <button type="button" class="menu_img_paper">
                                    <label for="<?= "menu_table_image_question_{$questionNo}" ?>">
                                        メニュー表添付
                                        <span class="file_name"></span>
                                    </label>
                                    <?= $this->Form->file('answer_file', ['id' => 'menu_table_image_question_' . $questionNo, 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                                    <?= $this->Form->hidden('menu_table_image', ['data-image-id' => 'menu_table_image_question_' . $questionNo, 'value' => '']); ?>
                                </button>
                                <button type="button" class="menu_img_store">
                                    <label for="<?= "store_image_question_{$questionNo}" ?>">
                                        店内のメニュー提示写真添付
                                        <span class="file_name"></span>
                                    </label>
                                    <?= $this->Form->file('answer_file', ['id' => 'store_image_question_' . $questionNo, 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                                    <?= $this->Form->hidden('store_image', ['data-image-id' => 'store_image_question_' . $questionNo, 'value' => '']); ?>
                                </button>
                            </div>
                        </div>
                    <?php endif ;?>

                    <?= $this->Form->hidden('menu_max_item', ['value' => $restaurantQuestion->menu_max_item]); ?>
                    <?= $this->Form->hidden('ingredient_max_item', ['value' => $restaurantQuestion->ingredient_max_item]); ?>
                <?php endif; ?>

                <?php if ($restaurantQuestion->type === TYPE_CHECKBOX) : ?>
                    <form id="select_form" enctype="multipart/form-data">
                        <div class="answer_checkbox_block">
                            <?php foreach ($restaurantQuestion->options as $key => $item) : ?>
                            <div class="answer_checkbox-txt-btn_box">
                                <label class="wrap_label">
                                    <input type="<?= h($restaurantQuestion->type) ?>" name="answers" value="<?= $key ?>">
                                    <p class="cb_text"><?= $item->label ?></p>
                                </label>
                            </div>
                            <?php endforeach; ?>
                        </div>

                        <?php if (property_exists($restaurantQuestion, 'effort_detail')) : ?>
                            <div class="text_area_block">
                                <textarea name="effort_detail" placeholder="取組内容" maxlength="1000"></textarea >
                            </div>
                        <?php endif; ?>

                        <div class="inner_box other_box">
                            <div class="wrap_menu_img_btn">
                                <?php if (property_exists($restaurantQuestion,'menu_table_image')) : ?>
                                    <button type="button" class="menu_img_paper">
                                        <label for="<?= "menu_table_image_question_{$questionNo}" ?>">
                                            メニュー表添付
                                            <span class="file_name"></span>
                                        </label>
                                        <?= $this->Form->file('answer_file', ['id' => 'menu_table_image_question_' . $questionNo, 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                                        <?= $this->Form->hidden('menu_table_image', ['data-image-id' => 'menu_table_image_question_' . $questionNo, 'value' => '']); ?>
                                    </button>
                                <?php endif; ?>

                                <?php if (property_exists($restaurantQuestion,'effort_image')) : ?>
                                    <button type="button" class="menu_img_store">
                                        <label for="<?= "effort_image_question_{$questionNo}" ?>">
                                            取り組みが確認できる写真添付
                                            <span class="file_name"></span>
                                        </label>
                                        <?= $this->Form->file('answer_file', ['id' => 'effort_image_question_' . $questionNo, 'class' => 'answer_menu_file', 'accept' => 'image/jpeg, image/png', 'hidden' => 'hidden']); ?>
                                        <?= $this->Form->hidden('effort_image', ['data-image-id' => 'effort_image_question_' . $questionNo, 'value' => '']); ?>
                                    </button>
                                <?php endif; ?>
                            </div>
                        </div>
                    </form>
                <?php endif; ?>

                <div class="next_btn_block">
                    <?php if ($questionNo === $total) : ?>
                        <p>
                            <button>
                                <?php
                                echo $this->Html->link('次へ', $this->Url->build([
                                    'prefix' => 'Restaurants',
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
                                        'prefix' => 'Restaurants',
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
                                        'prefix' => 'Restaurants',
                                        'controller' => 'Restaurant',
                                        'action' => 'index',
                                    ], ['fullBase' => false]));
                                    break;

                                default:
                                    echo $this->Html->link('戻る', $this->Url->build([
                                            'prefix' => 'Restaurants',
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
                            'prefix' => 'Restaurants',
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
