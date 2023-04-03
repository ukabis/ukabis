<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '生産者 申請済み 詳細ページ');?>
<section class="content farm_page">
    <article class="wrap_contents pb1rem">
        <section class="name_sec store_registered_detail">
            <h2><?= h($office->name) ?></h2>
        </section>

        <section class="contents_sec registered pt0pb0">
            <div class="contents_detail_title">
                <div class="contents_detail_title_flex">
                    <div class="status">
                        <p>認証：<?= h($office->certificate['rank_label']) ?></p>
                        <p>申請者：<?= h($office->business->representative_name) ?></p>
<!--                        <p>品名：静岡クラウンメロン</p>-->
<!--                        <p>地域：静岡市</p>-->
                    </div>
                    <?php if (!empty($office->certificate['mark_image'])): ?>
                        <p class="rank_img">
                            <img src="/<?= $office->certificate['mark_image'] ?>" alt="" />
                        </p>
                    <?php endif; ?>
                </div>
            </div>

            <div class="contents">
                <table class="registered_detail_table">
                    <tr>
                        <th class="pt0">業種</th>
                    </tr>
                    <tr>
                        <td>飲食</td>
                    </tr>
                    <tr>
                        <th>事業者名</th>
                    </tr>
                    <tr>
                        <td><?= h($office->business->name) ?></td>
                    </tr>
                    <tr>
                        <th>事業者名カナ</th>
                    </tr>
                    <tr>
                        <td><?= h($office->business->name_kana) ?></td>
                    </tr>
                    <tr>
                        <th>代表取締役</th>
                    </tr>
                    <tr>
                        <td><?= h($office->business->representative_name) ?></td>
                    </tr>
                    <tr>
                        <th>郵便番号</th>
                    </tr>
                    <tr>
                        <td><?= h($office->zip_code) ?></td>
                    </tr>
                    <tr>
                        <th>住所</th>
                    </tr>
                    <tr>
                        <td><?= h($office->prefecture) . h($office->city) . h($office->street) ?></td>
                    </tr>
                    <tr>
                        <th>電話番号</th>
                    </tr>
                    <tr>
                        <td><?= h($office->tel) ?></td>
                    </tr>
                    <tr>
                        <th>FAX番号</th>
                    </tr>
                    <tr>
                        <td><?= h($office->fax) ?></td>
                    </tr>
                    <tr>
                        <th>店舗名</th>
                    </tr>
                    <tr>
                        <td><?= h($office->name) ?></td>
                    </tr>
                    <tr>
                        <th>店舗名（カナ）</th>
                    </tr>
                    <tr>
                        <td><?= h($office->name_kana) ?></td>
                    </tr>
                    <?php if ($farmRequest): ?>
                    <tr>
                        <th>いまどき！リクエスト</th>
                    </tr>
                    <tr>
                        <td class="now_comment"><?= $farmRequest->comment ?></td>
                    </tr>
                    <tr>
                        <th>書き込み日時</th>
                    </tr>
                    <tr>
                        <td><?= $this->CustomHtml->dateformatJapan($farmRequest->created_at) ?></td>
                    </tr>
                    <?php endif; ?>
                    <tr>
                        <th>自己紹介</th>
                    </tr>
                    <tr>
                        <td class="self_comment"><?= h($office->offices_introduction) ?></td>
                    </tr>
                </table>

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
                <?php echo $this->Form->hidden('destination_office_id', ['value' => $office->id]); ?>
            </div>

            <div class="btn_detail_block">
                <p class="detail_back_btn">
                    <button>
                        <?= $this->Html->link('戻る', $this->Url->build([
                            'prefix' => 'Restaurants',
                            'controller' => 'Search',
                            'action' => 'list',
                            '?' => [
                                'page' => $page,
                                'area' => $area,
                                'keyword' => $keyword,
                            ]
                        ], ['fullBase' => true])) ?>
                    </button>
                </p>
                <p class="detail_edit_btn">
                    <button>
                        <?= $this->CustomHtml->linkMailto('お問い合わせする', [
                            'to_email' => $office->business->email ?? '',
                            'subject'  => MAIL_SUBJECT_INQUIRY
                        ]); ?>
                    </button>
                </p>
            </div>
        </section>
    </article>
</section>
