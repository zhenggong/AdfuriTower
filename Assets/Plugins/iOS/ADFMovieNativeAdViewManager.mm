//
//  ADFMovieNativeAdViewManager.m
//  Unity-iPhone
//
//  Created by Junhua Li on 2017/04/10.
//
//

#import "ADFMovieNativeAdViewManager.h"
#import "ADFMovieNativeAdViewUnityAdapter.h"
#import "ADFMovieUtilForUnity.h"

extern void UnitySendMessage(const char *, const char *, const char *);

extern "C" {
    void initializeMovieNativeAdViewIOS_(char* appID, char* pluginVersion, char* unityVersion);
    void loadMovieNativeAdViewIOS_(char* appID);
    void showMovieNativeAdViewIOS_(char* appID, float x, float y , float width, float height, float screenW);
    void setMovieNativeAdViewFrameIOS_(char* appID, float x, float y , float width, float height, float screenW);
    void playMovieNativeAdViewIOS_(char* appID);
    void hideMovieNativeAdViewIOS_(char* appID);
}
@interface ADFMovieNativeAdViewManager()<ADFmyMovieNativeDelegate, ADFMediaViewDelegate>
@end

@implementation ADFMovieNativeAdViewManager

static id<ADFmyMovieNativeDelegate, ADFMediaViewDelegate> instance = nil;
__strong static NSMutableDictionary* adMovieNativeAdList = nil;

static const char* UTILITY_GAMEOBJECT_NAME = "AdfurikunMovieNativeAdViewUtility";
static const char* UTILITY_FUNC_NAME = "MovieNativeAdViewCallback";

// こちら、NSLogを残したままご利用になられる方が多かったので、
// Debug の場合のみLogを表示するようにしています。
#define _ADF_DEBUG_ENABLE_
#ifdef _ADF_DEBUG_ENABLE_
#define adf_debug_NSLog(format, ...) NSLog(format, ## __VA_ARGS__)
#else
#define adf_debug_NSLog(format, ...)
#endif

- (id)init {
    self = [super init];
    if ( self ) {

    }
    return self;
}

+ (void)configureWithAppID:(NSString*)appId pluginVersion:(NSString*)pluginVersion unityVersion:(NSString*)unityVersion {
    if([appId length] < 1){return;}
    if ( instance == nil) {
        instance = [[ADFMovieNativeAdViewManager alloc] init];
    }

    if(adMovieNativeAdList == nil){
        adMovieNativeAdList = [@{} mutableCopy];
    }

    //iOSが対応バージョンなら、指定した広告枠で動画読み込みを開始
    if(![ADFmyMovieNative isSupportedOSVersion]){
        return;
    }
    NSDictionary *option = @{
                             @"plugin": @{
                                     @"type": @"unity",
                                     @"version": pluginVersion
                                     },
                             @"engine": @{
                                     @"type": @"unity",
                                     @"version": unityVersion
                                     }
                             };
    [ADFmyMovieNative configureWithAppID:appId option:option];

    //Unity用のアダプタークラスを生成
    ADFMovieNativeAdViewUnityAdapter* unityAdapter = [[ADFMovieNativeAdViewUnityAdapter alloc] initWithAppID:appId];
    //アダプターをリストに保存
    [adMovieNativeAdList setObject:unityAdapter forKey:appId];
}

+ (ADFMovieNativeAdViewUnityAdapter *)getAdapter:(NSString *)appID{
    ADFMovieNativeAdViewUnityAdapter* adapter = (ADFMovieNativeAdViewUnityAdapter *)[adMovieNativeAdList objectForKey:appID];
    return (adapter != nil) ? adapter: nil;
}

+ (void)loadMovieNativeAdView:(NSString*)appId{
    adf_debug_NSLog(@"loadMovieNativeAdView");
    ADFMovieNativeAdViewUnityAdapter* adapter = [ADFMovieNativeAdViewManager getAdapter:appId];
    if(adapter != nil){
        ADFmyMovieNative *movieNative = [adapter getMovieNative];
        [movieNative loadAndNotifyTo:instance];
    }
}

+ (void)showMovieNativeAdView:(NSString*)appId x:(float)x y:(float)y width:(float)width height:(float)height{
    adf_debug_NSLog(@"showMovieNativeAdView");
    [ADFMovieNativeAdViewManager setMovieNativeAdViewFrame:appId x:x y:y width:width height:height];
    [ADFMovieNativeAdViewManager playMovieNativeAdView:appId];
}

+ (void)setMovieNativeAdViewFrame:(NSString*)appId x:(float)x y:(float)y width:(float)w height:(float)h {
    adf_debug_NSLog(@"setMovieNativeAdView");
    ADFMovieNativeAdViewUnityAdapter* adapter = [ADFMovieNativeAdViewManager getAdapter:appId];
    if(adapter != nil){
        ADFMovieNativeAdInfo *adInfo = adapter.nativeAdInfo;
        if (adInfo != nil) {
            adInfo.mediaView.frame = CGRectMake(x, y, w, h);
        }
    }
}

+ (void)playMovieNativeAdView:(NSString*)appId{
    adf_debug_NSLog(@"playMovieNativeAdView");
    ADFMovieNativeAdViewUnityAdapter* adapter = [ADFMovieNativeAdViewManager getAdapter:appId];
    if(adapter != nil){
        ADFMovieNativeAdInfo *oldAdInfo = adapter.oldNativeAdInfo;
        if (oldAdInfo != nil) {
            [oldAdInfo.mediaView removeFromSuperview];
        }
        ADFMovieNativeAdInfo *adInfo = adapter.nativeAdInfo;
        if (adInfo != nil) {
            if (!adInfo.mediaView.superview) {
                UIView* unityView = UnityGetGLView();
                [unityView addSubview:adInfo.mediaView];
            }
            adInfo.mediaView.mediaViewDelegate = instance;
            [adInfo playMediaView];
        }
    }
}

+ (void)hideMovieNativeAdView:(NSString*)appId{
    adf_debug_NSLog(@"hideMovieNativeAdView");
    ADFMovieNativeAdViewUnityAdapter* adapter = [ADFMovieNativeAdViewManager getAdapter:appId];
    if(adapter != nil){
        ADFMovieNativeAdInfo *adInfo = adapter.nativeAdInfo;
        if (adInfo != nil) {
            [adInfo.mediaView removeFromSuperview];
        }
    }
}

#pragma mark - ADFmyMovieNativeDelegate

//ネイティブ広告の読み込み成功
- (void)onNativeMovieAdLoadFinish:(ADFMovieNativeAdInfo *)info appID:(NSString *)appID {
    ADFMovieNativeAdViewUnityAdapter* adapter = [ADFMovieNativeAdViewManager getAdapter:appID];
    if(adapter != nil){
        [adapter setNativeAdInformation:info];
    }

    adf_debug_NSLog(@"onNativeMovieAdViewLoadFinish");
    UnitySendMessage( UTILITY_GAMEOBJECT_NAME, UTILITY_FUNC_NAME,
                     [ADFMovieUtilForUnity convUnityParamFormat:@"LoadFinish" appID:appID ]
                     );
}

//ネイティブ広告の読み込み失敗
- (void)onNativeMovieAdLoadError:(ADFMovieError *)error appID:(NSString *)appID {
    NSLog(@"Failed to load native ad, error code=%lu, error message=\"%@\"", (unsigned long)error.errorCode, error.errorMessage);
    ADFMovieNativeAdViewUnityAdapter* adapter = [ADFMovieNativeAdViewManager getAdapter:appID];
    if(adapter != nil){
        adapter.nativeAdInfo = nil;
    }

    adf_debug_NSLog(@"onNativeMovieAdViewLoadError");
    UnitySendMessage( UTILITY_GAMEOBJECT_NAME, UTILITY_FUNC_NAME,
                     [ADFMovieUtilForUnity convUnityParamFormat:@"LoadError" appID:appID errCode:[NSString stringWithFormat:@"%ld", (long)error.errorCode]]
                     );
}

#pragma mark - ADFMediaViewDelegate
- (void)onADFMediaViewPlayStart {
    adf_debug_NSLog(@"onNativeMovieAdViewPlayStart");
    UnitySendMessage( UTILITY_GAMEOBJECT_NAME, UTILITY_FUNC_NAME,
                     [ADFMovieUtilForUnity convUnityParamFormat:@"PlayStart" appID:@"" ]
                     );
}

- (void)onADFMediaViewPlayFinish {
    adf_debug_NSLog(@"onNativeMovieAdViewPlayFinish");
    UnitySendMessage( UTILITY_GAMEOBJECT_NAME, UTILITY_FUNC_NAME,
                     [ADFMovieUtilForUnity convUnityParamFormat:@"PlayFinish" appID:@"" isVideo:@"1"]
                     );
}

- (void)onADFMediaViewPlayFail {
    adf_debug_NSLog(@"onNativeMovieAdViewLoadError");
    UnitySendMessage( UTILITY_GAMEOBJECT_NAME, UTILITY_FUNC_NAME,
                     [ADFMovieUtilForUnity convUnityParamFormat:@"PlayError" appID:@"" errCode:[NSString stringWithFormat:@"%ld", 0]]
                     );
}

@end

void initializeMovieNativeAdViewIOS_(char* appID, char* pluginVersion, char* unityVersion){
    [ADFMovieNativeAdViewManager configureWithAppID:[NSString stringWithCString:appID encoding:NSUTF8StringEncoding]
                                      pluginVersion:[NSString stringWithCString:pluginVersion encoding:NSUTF8StringEncoding]
                                       unityVersion:[NSString stringWithCString:unityVersion encoding:NSUTF8StringEncoding]];
}

void loadMovieNativeAdViewIOS_(char* appID){
    [ADFMovieNativeAdViewManager loadMovieNativeAdView:[NSString stringWithCString:appID encoding:NSUTF8StringEncoding]];
}

void showMovieNativeAdViewIOS_(char* appID, float x, float y, float width, float height, float screenW) {
    float scale = UIScreen.mainScreen.bounds.size.width / screenW;
    float _w = width * scale;
    float _h = height * scale;
    float _x = x * scale;
    float _y = y * scale;

    NSString *_appID = [NSString stringWithCString:appID encoding:NSUTF8StringEncoding];
    [ADFMovieNativeAdViewManager showMovieNativeAdView:_appID x:_x y:_y width:_w height:_h];
}

void setMovieNativeAdViewFrameIOS_(char* appID, float x, float y, float width, float height, float screenW) {
    float scale = UIScreen.mainScreen.bounds.size.width / screenW;
    float _w = width * scale;
    float _h = height * scale;
    float _x = x * scale;
    float _y = y * scale;

    NSString *_appID = [NSString stringWithCString:appID encoding:NSUTF8StringEncoding];
    [ADFMovieNativeAdViewManager setMovieNativeAdViewFrame:_appID x:_x y:_y width:_w height:_h];
}

void playMovieNativeAdViewIOS_(char* appID){
    [ADFMovieNativeAdViewManager playMovieNativeAdView:[NSString stringWithCString:appID encoding:NSUTF8StringEncoding]];
}

void hideMovieNativeAdViewIOS_(char* appID){
    [ADFMovieNativeAdViewManager hideMovieNativeAdView:[NSString stringWithCString:appID encoding:NSUTF8StringEncoding]];
}
