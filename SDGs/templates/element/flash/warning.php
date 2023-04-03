<?php
/**
 * @var \App\View\AppView $this
 * @var array $params
 * @var string $message
 */
if (!isset($params['escape']) || $params['escape'] !== false) {
    $message = h($message);
}
?>
<div class="flash-message">
    <div class="message warning" onclick="this.classList.add('hidden');">
        <p class="text"><?= $message ?></p>
    </div>
</div>