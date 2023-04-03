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
<?php $this->assign('title', '生産者 飲食店検索一覧ページ') ?>
<section class="content">
    <div class="wrap_page">
		<article class="wrap_contents">
		<section class="name_sec request_edit">
			<h2>いまどき！リクエスト一覧</h2>
		</section>
			<section class="contents_sec registered pt0pb0 edit request_list request_search_list">
				<div class="contents_edit_title farm_search_store">
				<h3>店舗を探す</h3>
				</div>
				<div class="contents">
					<div class="edit_input">
					<?php if (count($resultRestaurantRequests)): ?>
						<?php $listTitle = []; ?>
						<?php foreach ($resultRestaurantRequests as $keyRequest => $request): ?>
							<?php
							$title = [];
							foreach ($dataFarmRequests as $key => $item) {
								if(str_contains(strtoupper($request->food), strtoupper($item->food))) {
									array_push($title, $item->food);
								}
							}
							$listTitle[$keyRequest] = $title;
							if ($keyRequest == 0) {
								echo "<h4 class='request_title'>" . implode("／", $title) . "</h4>";
							}
							elseif ($keyRequest > 0 && $listTitle[$keyRequest] != $listTitle[$keyRequest - 1]) {
								echo "<h4 class='request_title'>" . implode("／", $title) . "</h4>";
							}
							?>
							<p class="request_edit_input">
							<div class="request_registered_detail">
								<?= $this->Html->link($request->office->name .' ｜ '. $request->food .' ｜ '. $request->created_at->format('Y/m/d'), $this->Url->build([
									'prefix' => 'Farms',
									'controller' => 'RestaurantRequest',
									'action' => 'detail',
									'requestId' => $request->id,
									'?' => ['page' => $this->Paginator->param('page')],
								], ['fullBase' => true])) ?>
							</div>
							<p><input type="text" class="request_comment disabled" readonly placeholder="<?= $request->comment ?>" value=""></p>
						<?php endforeach; ?>
						<div class="btn_edit_block btn_request_Search">
							<p class="edit_back_btn">
								<button>
								<?= $this->Html->link('戻る', $this->Url->build([
									'prefix' => 'Farms',
									'controller' => 'Request',
									'action' => 'list'
								], ['fullBase' => true])) ?>
								</button>
							</p>
							<p class="edit_page_btn">
								<button>
									<?= $this->Paginator->prev('<'); ?>
									<?= $this->Paginator->counter('PAGE {{page}}/{{pages}}'); ?>
									<?= $this->Paginator->next('>'); ?>
								</button>
							</p>
						</div>
						<?php else: ?>
							<p class="text-center">該当データが存在しません。</p>
							<div class="btn_edit_block btn_request_Search">
								<p class="edit_back_btn">
									<button>
									<?= $this->Html->link('戻る', $this->Url->build([
										'prefix' => 'Farms',
										'controller' => 'Request',
										'action' => 'list'
									], ['fullBase' => true])) ?>
									</button>
								</p>
							</div>
						<?php endif; ?>

					</div>
				</div>
			</section>
		</article>
    </div>
</section>
