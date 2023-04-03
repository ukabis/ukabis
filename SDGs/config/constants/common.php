<?php

define('BASE_UPLOAD_FOLDER', 'uploads');
define('RESTAURANT_PUBLIC_FILE_PATH', BASE_UPLOAD_FOLDER . DS . 'restaurants' . DS);
define('PRODUCER_PUBLIC_FILE_PATH', BASE_UPLOAD_FOLDER . DS . 'producers' . DS);

define('CSS_CERTIFICATION_PATH', 'css/certification.css');
define('BASE_CERTIFICATION_FOLDER',  'certifications');
define('RESTAURANT_CERTIFICATION_FILE_PATH', BASE_CERTIFICATION_FOLDER . DS . 'restaurants' . DS);
define('PRODUCER_CERTIFICATION_FILE_PATH', BASE_CERTIFICATION_FOLDER . DS . 'producers' . DS);
define('BACKGROUND_CERTIFICATION_PDF', 'images/certification/bg_certificate.png');
define('FARM_GOLD_MARK_IMAGE', 'images/certification/farm_mark/gold.png');
define('FARM_SILVER_MARK_IMAGE', 'images/certification/farm_mark/silver.png');
define('FARM_BRONZE_MARK_IMAGE', 'images/certification/farm_mark/bronze.png');
define('RESTAURANT_GOLD_MARK_IMAGE', 'images/certification/restaurant_mark/gold.png');
define('RESTAURANT_SILVER_MARK_IMAGE', 'images/certification/restaurant_mark/silver.png');
define('RESTAURANT_BRONZE_MARK_IMAGE', 'images/certification/restaurant_mark/bronze.png');

define('OFFICE_RESTAURANT', 1);
define('OFFICE_PRODUCER', 2);
define('MAX_UPLOAD_FILESIZE', '50MB');
define('TYPES_ALLOW_UPLOAD', ['image/png', 'image/jpg', 'image/jpeg']);
define('REGEX_PASSWORD', '/^(?=.*[A-Z])(?=.*[a-z])(?=.*[#@\-!$%^&*()_+|~=`{}\[\]:";\'<>?,.\/])[A-Za-z\d#@\-!$%^&*()_+|~=`{}\[\]:";\'<>?,.\/]{8,16}$/');
define('MAX_MENU_QUESTION_1', 10);
define('MAX_MENU_QUESTION_7', 5);
