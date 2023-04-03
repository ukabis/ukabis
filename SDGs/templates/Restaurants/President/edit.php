<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '飲食店 編集ページ');?>
<section class="content">
    <article class="wrap_contents pb1rem">
        <section class="name_sec registered_edit">
            <h2>企業/店舗情報編集</h2>
        </section>
        <section class="contents_sec registered pt0pb0 edit">
            <?= $this->Form->create(['office' => $office, 'business' => $business], ['type' => 'file']); ?>
            <div class="contents">
            <h4 class="ragist_title">企業情報を編集</h4>
            <div class="edit_input">
                <?php
                    echo $this->Form->control('business.name', ['placeholder' => '企業名', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('business.name_kana', ['placeholder' => '企業名（カナ）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('business.zip_code', ['type' => 'text', 'placeholder' => '郵便番号（ハイフン不要）', 'label' => false, 'required' => false, 'maxlength' => 20, 'class' => 'input-number']);
                    echo $this->Form->control('business.prefecture', ['placeholder' => '住所1（〇〇県）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('business.city', ['placeholder' => '住所2（〇〇市〇〇町）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('business.street', ['placeholder' => '住所3（丁目 番地 号 〇〇ビル 〇号室）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('business.representative_name', ['placeholder' => '代表者名', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('business.tel', ['type' => 'text', 'placeholder' => '電話番号（ハイフン不要）', 'label' => false, 'required' => false, 'maxlength' => 20, 'class' => 'input-number']);
                    echo $this->Form->control('business.fax', ['placeholder' => 'FAX番号（ハイフン不要）', 'label' => false, 'required' => false, 'maxlength' => 20]);
                    echo $this->Form->control('business.homepage_url', ['placeholder' => 'WebサイトURL', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('business.email', ['placeholder' => 'メールアドレス（会社用）', 'label' => false, 'required' => false, 'maxlength' => 255, 'type' => 'text']);
                ?>
            </div>
            </div>
            <div class="contents">
            <h4 class="ragist_title">店舗情報を編集</h4>
            <div class="edit_input">
                <?php
                    echo $this->Form->control('office.name', ['placeholder' => '店舗名', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('office.name_kana', ['placeholder' => '店舗名（カナ）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('office.zip_code', ['type' => 'text', 'placeholder' => '郵便番号（ハイフン不要）', 'label' => false, 'required' => false, 'maxlength' => 20, 'class' => 'input-number']);
                    echo $this->Form->control('office.prefecture', ['placeholder' => '住所1（〇〇県）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('office.city', ['placeholder' => '住所2（〇〇市〇〇町）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('office.street', ['placeholder' => '住所3（丁目 番地 号 〇〇ビル 〇号室）', 'label' => false, 'required' => false, 'maxlength' => 255]);
                    echo $this->Form->control('office.tel', ['type' => 'text', 'placeholder' => '電話番号（ハイフン不要）', 'label' => false, 'required' => false, 'maxlength' => 20, 'class' => 'input-number']);
                    echo $this->Form->control('office.fax', ['placeholder' => 'FAX番号（ハイフン不要）', 'label' => false, 'required' => false, 'maxlength' => 20]);
                    echo $this->Form->control('office.offices_introduction', ['type' => 'textarea', 'placeholder' => '店舗自己紹介', 'label' => false, 'required' => false, 'class' => 'store_introduce', 'maxlength' => 1000]);
                    echo '<p class="ragistration_img_btn">' . $this->Form->label('ragistration-img-btn-1', $office->profile_image_1 ? '店舗自己紹介画像 1(' . str_replace('uploads/restaurants/', '', $office->profile_image_1) . ')' : '店舗自己紹介画像 1', [
                        'class' => 'ragistration_img_btn'
                    ]) . '</p>';
                    echo $this->Form->control('office.file_upload_1', ['type' => 'file', 'placeholder' => '店舗メッセージ', 'label' => false, 'required' => false, 'id' => 'ragistration-img-btn-1', 'style' => 'display: none;', 'accept' => 'image/png, image/jpg']);
                    echo '<p class="ragistration_img_btn">' . $this->Form->label('ragistration_img_btn_2', $office->profile_image_2 ? '店舗自己紹介画像 2(' . str_replace('uploads/restaurants/', '', $office->profile_image_2) . ')' : '店舗自己紹介画像 2', [
                        'class' => 'ragistration_img_btn'
                    ]) . '</p>';
                    echo $this->Form->control('office.file_upload_2', ['type' => 'file', 'placeholder' => '店舗メッセージ', 'label' => false, 'required' => false, 'id' => 'ragistration-img-btn-2', 'style' => 'display: none;', 'accept' => 'image/png, image/jpg']);
                ?>
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
            <p class="edit_save_btn"><?= $this->Form->button('更新する'); ?></p>
            </div>
            <?= $this->Form->end(); ?>
        </section>
    </article>
</section>

