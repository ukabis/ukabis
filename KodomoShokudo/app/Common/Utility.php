<?php
namespace App\Common;


use Illuminate\Support\Facades\Log;

class Utility
{

    public static function translate_to_jp($en_string){
        if(isset(Constant::TRANSLATE_MAP_1[$en_string])){
            return Constant::TRANSLATE_MAP_1[$en_string];
        }
        foreach (Constant::TRANSLATE_MAP_2 as $key => $value){
            if (preg_match($key, $en_string, $matches)) {
                if(count($matches) >= 2){
                    return str_replace('[1]', $matches[1], Constant::TRANSLATE_MAP_2[$key]);
                }
            }
        }
        Log::error('translate_to_jp: ' . $en_string . 'が日本語に翻訳できません。');
        return $en_string;

    }
}