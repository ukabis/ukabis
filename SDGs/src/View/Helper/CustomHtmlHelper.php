<?php

namespace App\View\Helper;

use Cake\I18n\FrozenTime;
use Cake\View\Helper\HtmlHelper;

/**
 * Html Helper class for easy use of HTML widgets.
 *
 * HtmlHelper encloses all methods needed while working with HTML pages.
 *
 * @property \Cake\View\Helper\UrlHelper $Url
 * @link https://book.cakephp.org/4/en/views/helpers/html.html
 */
class CustomHtmlHelper extends HtmlHelper
{
    /*
    * @param array|string $path Path to the image file, relative to the webroot/uploads/ directory.
    * @param array<string, mixed> $options Array of HTML attributes. See above for special options.
    * @return string completed img tag
    * @link https://book.cakephp.org/4/en/views/helpers/html.html#linking-to-images
    */
    public function imageUpload($path, array $options = []): string
    {
        if (is_string($path)) {
            $path = $this->Url->assetUrl($path, $options);
        } else {
            $path = $this->Url->build($path, $options);
        }
        $options = array_diff_key($options, ['fullBase' => null, 'pathPrefix' => null]);

        if (!isset($options['alt'])) {
            $options['alt'] = '';
        }

        $url = false;
        if (!empty($options['url'])) {
            $url = $options['url'];
            unset($options['url']);
        }

        $templater = $this->templater();
        $image = $templater->format('image', [
            'url' => $path,
            'attrs' => $templater->formatAttributes($options),
        ]);

        if ($url) {
            return $templater->format('link', [
                'url' => $this->Url->build($url),
                'attrs' => null,
                'content' => $image,
            ]);
        }

        return $image;
    }

    /*
    * Format date to japan
    * @param $date Datetime
    */
    public function dateformatJapan($date): string
    {
        if (!strtotime($date)) {
            return '';
        }

        $now = new FrozenTime($date);
        $dy  = $date->format('w');
        $dys = ["日","月","火","水","木","金","土"];

        return sprintf(
            '%s年%s月%s日 (%s) %s:%s',
            $now->i18nFormat('Y'),
            $now->i18nFormat('M'),
            $now->i18nFormat('d'),
            $dys[$dy],
            $now->i18nFormat('HH'),
            $now->i18nFormat('mm')
        );
    }

    /**
     * Cut the string by length and show shortened string
     *
     * @param string|null $text
     * @param int $maxlength
     *
     * @return string
     */
    public function renderTextWithMaxLength(string|null $text, int $maxlength = 8): string
    {
        $content = $text;
        $templater = $this->templater();
        $options = [
            'class' => 'ellipsis-text'
        ];

        if (count(mb_str_split($content)) > $maxlength) {
            $options['data-content'] = $content;
            $content = implode('', array_slice(mb_str_split($content), 0, $maxlength)) . '...';
        }
        $span = $templater->format('tag', [
            'tag' => 'span',
            'content' => $content,
            'attrs' => $templater->formatAttributes($options),
        ]);

        return $span;
    }

    /**
     * Custom link href mail to
     *
     * @param string $linkTitle
     * @param array $options
     *
     * @return string
     */
    public function linkMailto(string $linkTitle, array $options = []): string
    {
        $to_email = '';
        $params = '?';

        if (isset($options['to_email'])) {
            $to_email = $options['to_email'];
        }

        if (isset($options['subject'])) {
            $subject = $options['subject'];
            $params .= 'subject=' . $subject . '&';
        }

        if (isset($options['cc'])) {
            $cc = $options['cc'];
            $params .= 'cc=' . $cc . '&';
        }

        if (isset($options['bcc'])) {
            $bcc = $options['bcc'];
            $params .= 'bcc=' . $bcc . '&';
        }

        if (isset($options['body'])) {
            $body = $options['body'];
            $params .= 'body=' . $body . '&';
        }

        return $this->link($linkTitle, sprintf("mailto:%s%s", $to_email, $params));
    }
}
