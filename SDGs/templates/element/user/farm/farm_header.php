<?php
$this->loadHelper('Authentication.Identity');
?>
<?php if ($this->Identity->isLoggedIn()): ?>
	<header>
        <h1>ふじのくにSDGs認証 申請システム</h1>
        <div class="menu_btn">
            <span class="nav_toggle">
            <i></i>
            <i></i>
            <i></i>
            </span>
        </div>
        <nav class="nav">
            <ul class="nav_menu_ul">
                <li class="nav_menu_li"><?= $this->Html->link('生産者マイページ', '/farms', ['class' => 'button']); ?></li>
                <li class="nav_menu_li">
                    <?= $this->Html->link('飲食店を検索する', '/farms/search', ['class' => 'button']); ?>
                </li>
            <li class="nav_menu_li"><?= $this->Html->link('ログアウト', '/farms/logout', ['class' => 'button']); ?></li>
            </ul>
        </nav>
    </header>
<?php else: ?>
	<header>
        <h1>ふじのくにSDGs認証申請システム</h1>
        <div class="menu_btn">
            <span class="nav_toggle">
            <i></i>
            <i></i>
            <i></i>
            </span>
        </div>
        <nav class="nav">
            <ul class="nav_menu_ul">
                <li class="nav_menu_li"><?= $this->Html->link('トップに戻る', '/', ['class' => 'button']); ?></li>
            </ul>
        </nav>
	</header>
<?php endif; ?>
