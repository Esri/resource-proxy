<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper;

/**
 * Description of UrlHelper
 *
 * @author scharfenberg
 */
abstract class UrlHelper {
    
    static public function buildRestMethodUrl($resourceUrl, $method, $query = null) {
        
        $joinedUrl = http_build_url('',
                [   'host' => $resourceUrl,
                    'path' => $method,
                    'query' => $query],
                HTTP_URL_JOIN_PATH | HTTP_URL_JOIN_QUERY);
        
        return $joinedUrl;
    }
    
}
