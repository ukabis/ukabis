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
					<li>
                        <?= $this->Html->link('管理者メニュー', $this->Url->build(['_name' => 'admin.index'], ['fullBase' => true])) ?>
                    </li>
					<li>
                        <?= $this->Html->link('管理者アカウント 一覧', $this->Url->build(['_name' => 'admin.list'], ['fullBase' => true])) ?>
                    </li>
					<li>
                        <?= $this->Html->link('認証者アカウント 一覧', $this->Url->build(['_name' => 'admin.checker.list'], ['fullBase' => true])) ?>
                    </li>
					<li>
                        <?= $this->Html->link('ログアウト', $this->Url->build(['_name' => 'admin.logout'], ['fullBase' => true]), ['class' => 'button']) ?>
                    </li>
				</ul>
			</div>
			<p class="logout">
                <button>
                    <?= $this->Html->link('log out', $this->Url->build(['_name' => 'admin.logout'], ['fullBase' => true]), ['class' => 'button']) ?>
                </button>
            </p>
			<p class="mypage">
				<?= $this->Html->link(
					$this->Html->image('../images/icon_mypage.png'),
					[
						'controller' => 'Admin',
						'action' => 'index'
					], ['escape' => false]
				); ?>
			</p>
		</div>
	</header>
<?php else: ?>
	<header class="admin_top">
		<div class="header_inner">
			<h1>ふじのくにSDGs認証 申請システム</h1>
		</div>
	</header>
<?php endif; ?>
