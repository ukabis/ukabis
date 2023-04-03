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
<?php $this->assign('title', '認証飲食店 検索結果 一覧ページ');?>
<section class="content">
<article class="wrap_contents">
      <section class="contents_sec registered">
        <div class="contents_title">
          <h2>認証飲食店一覧</h2>
        </div>
        <div class="contents">
          <div class="wrap_registered_menu">
            <div class="registered_menu_title">
				<div class="registered_name_title user">事業者名</div>
				<!-- <div class="registered_rank_title">認証</div> -->
            </div>

            <?php if (count($offices)): ?>
				<?php foreach ($offices as $office): ?>
				<div class="registered_menu_box">
					<div class="registered_name_detail user">
						<a href="<?= '/search/restaurant/'. $office->id .'?' . $query ?>"><?= h($office->name) ?></a>
					</div>
					<!-- <div class="registered_rank_detail"><?= h($office->certificate['rank_label']) ?></div> -->
				</div>
				<?php endforeach; ?>
            <?php else: ?>
			<div class="registered_menu_box">
				<div class="registered_name_detail empty">登録がありません。</div>
			</div>
            <?php endif; ?>
          </div>

          <div class="btn_paging_block">
            <p class="registered_btn">
              <button class="btn_color_orange">
					<a href="<?= '/home' .'?' . $query ?>">戻る</a>
              </button>
            </p>
			<p class="registered_paging">
				<button>
					<?= $this->Paginator->prev('<'); ?>
					<?= $this->Paginator->counter('PAGE {{page}}/{{pages}}', ['class' => 'paging_number']); ?>
					<?= $this->Paginator->next('>'); ?>
				</button>
			</p>
          </div>
        </div>
      </section>
    </article>
</section>
