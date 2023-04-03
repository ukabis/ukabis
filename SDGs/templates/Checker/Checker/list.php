<?php
/**
 * @var \App\View\AppView $this
 */
$this->loadHelper('Authentication.Identity');
?>
<?php $this->assign('title', '認証者 一覧');?>
<section class="content">
	<article class="contents">
		<section class="title_sec">
			<h2>認証者 一覧</h2>
		</section>

		<section class="list_sec">
			<table class="table-responsive">
				<tr>
					<th>ID</th>
					<th>氏名</th>
					<th>メールアドレス</th>
					<th>Actions</th>
				</tr>
				<?php foreach ($listChecker as $item) : ?>
				<tr>
					<td><?= $item->id ?></td>
                    <td><?= $this->CustomHtml->renderTextWithMaxLength($item->name) ?></td>
                    <td><?= $this->CustomHtml->renderTextWithMaxLength($item->email, 20) ?></td>
					<td>
                        <?= $this->Html->link('閲覧', $this->Url->build([
                            'prefix' => 'Checker',
                            'controller' => 'Checker',
                            'action' => 'detail',
                            'id' => $item->id
                        ], ['fullBase' => true])) ?>
                        <?= $this->Html->link('編集', $this->Url->build([
                            'prefix' => 'Checker',
                            'controller' => 'Checker',
                            'action' => 'edit',
                            'id' => $item->id
                        ], ['fullBase' => true])) ?>
						<?php if ($user->id != $item->id): ?>
							<?= $this->Html->link('削除', $this->Url->build([
                                'prefix' => 'Checker',
                                'controller' => 'Checker',
                                'action' => 'delete',
                                'id' => $item->id
                            ], ['fullBase' => true])) ?>
						<?php else: ?>
							<span></span>
						<?php endif; ?>
					</td>
				</tr>
			<?php endforeach; ?>
        	</table>
      	</section>

		<?= $this->element('paginate/jump_links'); ?>

		<section class="btn_sec">
			<p>
				<button class="backBtn">
					<?= $this->Html->link('ホームに戻る', $this->Url->build([
						'prefix' => 'Checker',
						'controller' => 'Checker',
						'action' => 'index',
					], ['fullBase' => true])) ?>
				</button>
			</p>
		</section>

		<?= $this->element('paginate/page_counter'); ?>
    </article>
</section>

