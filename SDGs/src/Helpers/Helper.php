<?php

if (!function_exists('array_flatten')) {
    /**
     * Convert a multi-dimensional array into a single-dimensional array.
     * @author Sean Cannon, LitmusBox.com | seanc@litmusbox.com
     * @param  array $array The multi-dimensional array.
     * @return array
     */
    function array_flatten($array)
    {
        if (!is_array($array)) {
            return false;
        }
        $result = array();
        foreach ($array as $key => $value) {
            if (is_array($value)) {
                $result = array_merge($result, array_flatten($value));
            } else {
                $result = array_merge($result, array($key => $value));
            }
        }

        return $result;
    }
}

if (!function_exists('cols_from_array')) {
    /**
     * Convert a array into a array with multple column belongs to the old array.
     *
     * @param array $array
     * @param $keys
     * @return array
     */
    function cols_from_array(array $array, $keys)
    {
        if (!is_array($keys)) $keys = [$keys];

        return array_map(function ($el) use ($keys) {
            $o = [];

            foreach ($keys as $key) {
                $o[$key] = isset($el[$key]) ? $el[$key] : false;
            }

            return $o;
        }, $array);
    }
}
