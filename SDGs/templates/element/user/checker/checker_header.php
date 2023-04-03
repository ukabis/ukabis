<?php
$this->loadHelper('Authentication.Identity');
?>
<?php if ($this->Identity->isLoggedIn()): ?>
	<header>
		<div class="header_inner">
			<h1>ふじのくにSDGs認証 申請システム</h1>
			<p class="menu"><a href="#">ページ一覧</a></p>
			<div class="menu_list">
				<ul>
				<li><?= $this->Html->link('認証者 ホーム', '/checker', ['class' => 'button']); ?></li>
				<li><?= $this->Html->link('認証者 一覧', '/checker/list', ['class' => 'button']); ?></li>
				<li><?= $this->Html->link('ログアウト', '/checker/logout', ['class' => 'button']); ?></li>
				</ul>
			</div>
			<p class="logout"><button><?= $this->Html->link('log out', '/checker/logout', ['class' => 'button']); ?></button></p>
			<p class="mypage">
				<?= $this->Html->link(
					$this->Html->image('../images/icon_mypage.png'),
					[
						'controller' => 'Checker', 
						'action' => 'index'
					], ['escape' => false]
				); ?>
			</p>
		</div>
	</header>
<?php else: ?>
	<header class="shinsa_top">
		<div class="header_inner">
			<h1>ふじのくにSDGs認証 申請システム</h1>
		</div>
	</header>
<?php endif; ?>
