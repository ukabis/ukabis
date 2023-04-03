FROM php:8.0.5-fpm

RUN apt-get update

RUN apt-get install -y libzip-dev zip unzip libpng-dev libjpeg-dev zlib1g-dev procps vim libxml2-dev --no-install-recommends
RUN docker-php-ext-configure gd --with-jpeg
# Install composer
RUN curl -sS https://getcomposer.org/installer | php -- --install-dir=/usr/local/bin --filename=composer

# Install php ext
RUN docker-php-ext-install pdo pdo_mysql zip exif gd intl

CMD php-fpm