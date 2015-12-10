<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

/**
 * Description of AdminNotice
 *
 * @author scharfenberg
 */
abstract class AdminNotice {
    
    /**
     * Registers a Wordpress admin notice.
     * @param callable $checker This callback function returns the html class
     *      to be used for the message (e.g. 'error', 'notice' ...). If it
     *      return falsy no message is displayed.
     * @param string/callable $msg The message to be displayed. If a callback
     *      has been specified call it first without any arguments and use the
     *      result as message. This feature can be used to provide localized
     *      messages.
     */
    static public function register($checker, $pluginName, $msg) {
        
        if(is_callable($checker)) {
            add_action('admin_notices',
                    function() use ($checker, $pluginName, $msg) {
                        $class = call_user_func($checker);
                        if($class) {
                            self::renderMessage($msg, $pluginName, $class);
                        }
            });
        }
    }
    
    static private function renderMessage($msg, $pluginName, $class) {
        
        if(is_callable($msg)) {
            $msg = call_user_func($msg);
        }
        
        $html = <<<EOT
                <div class='{$class} notice is-dismissible'>
                    <h3>Plugin: $pluginName</h3>
                    <p>$msg</p>
                </div>
EOT;
                
        echo $html;
    }
}
