<p>
    <label for="<?php echo $widgetSettings->serviceUrlHtmlId(); ?>">
        <?php _ex('OnlineRateTable Service Url', 'Widget Settings', 'FpOnlineRateTable'); ?>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->serviceUrlHtmlId(); ?>"
           name="<?php echo $widgetSettings->serviceUrlHtmlName(); ?>"
           value="<?php echo $widgetSettings->serviceUrl(); ?>" />
</p>