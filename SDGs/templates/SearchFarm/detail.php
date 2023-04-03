<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '飲食店 申請済み 詳細ページ') ?>
<section class="content farm_page">
    <article class="wrap_contents pb1rem">
        <section class="name_sec store_registered_detail">
            <h2><?= h($office->name) ?></h2>
        </section>

        <section class="contents_sec registered search_content pt0pb0">
            <div class="contents_detail_title search_content_detail">
                <div class="contents_detail_title_flex">
                    <div class="status">
                        <p>認証：<?= h($office->certificate['rank_label']) ?></p>
                        <?php if ($farmRequest): ?>
                        <p>品名：<?= $farmRequest->food ?></p>
                        <?php endif; ?>
                    </div>
                    <?php if (!empty($office->certificate['mark_image'])): ?>
                        <p class="rank_img">
                            <img src="/<?= $office->certificate['mark_image'] ?>" alt="" />
                        </p>
                    <?php endif; ?>
                </div>
                <table class="contents_rank">
                    <tr>
                        <th>郵便番号：</th>
                        <td><?= $office->zip_code ?></td>
                    </tr>
                    <tr>
                        <th>住所：</th>
                        <td><?= h($office->prefecture) . h($office->city) . h($office->street); ?></td>
                    </tr>
                    <tr>
                        <th>電話番号：</th>
                        <td><?= $office->tel ?></td>
                    </tr>
                    <tr>
                        <th>FAX番号：</th>
                        <td><?= $office->fax ?></td>
                    </tr>
                    <tr>
                        <th>URL：</th>
                        <td><?= $office->business->homepage_url ?></td>
                    </tr>
            </table>
            </div>

            <div class="contents">
                <div class="pr_comment">
                    <p><?= h($office->offices_introduction) ?></p>
                </div>

                <div class="<?= $office && $office->profile_image_1 && $office->profile_image_2 ? 'pic_block' : 'pic_block_single'?>" id="pic_block_images">
                    <?php if ($office && $office->profile_image_1) : ?>
                        <p>
                            <?= $this->CustomHtml->imageUpload(h($office->profile_image_1), ['alt' => '店舗自己紹介画像 1', 'id' => 'profile_img_1']); ?>
                        </p>
                    <?php endif; ?>

                    <?php if ($office && $office->profile_image_2) : ?>
                        <p>
                            <?= $this->CustomHtml->imageUpload(h($office->profile_image_2), ['alt' => '店舗自己紹介画像 2', 'id' => 'profile_img_2']); ?>
                        </p>
                    <?php endif; ?>

                    <?php if (!$office->profile_image_1 && !$office->profile_image_2) : ?>
                        <p>
                            <?= $this->Html->image("img_no-image.png"); ?>
                        </p>
                    <?php endif; ?>
                </div>

                <?= $this->Form->hidden('destination_office_id', ['value' => $office->id]); ?>
            </div>

            <div class="btn_detail_block">
                <p class="detail_back_btn">
                    <button>
						<a href="<?= '/search/farm' .'?' . $query ?>">戻る</a>
                    </button>
                </p>
            </div>

        </section>
    </article>
</section>
