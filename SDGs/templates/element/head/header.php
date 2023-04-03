<?php

$login = $this->request->getSession()->read('Auth');
if ($login["email"] !=null){
    echo ('<div style="margin: 5px;float: left;">');
    echo ('<div style="clear: both"></div>');
}
