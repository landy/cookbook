module Cookbook.Client.View

open Cookbook.Client.State

open Fable.React
open Feliz
open Feliz.Router
open Feliz.MaterialUI
open Cookbook.Client.Router
open Cookbook.Client.Pages

module Html =
    module Props =
        let routed (p:Page) =
            [
                prop.href (p |> Page.toUrlSegments |> Router.formatPath)
                prop.onClick (Router.goToUrl)
            ]

    let aRouted (text:string) (p:Page) =
        Html.a [
            yield! Props.routed p
            prop.text text
        ]

let private useToolbarStyles = Styles.makeStyles(fun styles theme ->
  {|
    appBarTitle = styles.create [
      style.flexGrow 1
    ]
  |}
)

let private useRootViewStyles : unit -> _ = Styles.makeStyles(fun styles theme ->
  let drawerWidth = 240
  {|
    root = styles.create (fun model -> [
      style.display.flex
      style.userSelect.none
    ])
    appBar = styles.create [
      style.zIndex (theme.zIndex.drawer + 1)
    ]
    drawer = styles.create [
      style.width (length.px drawerWidth)
      style.flexShrink 0
    ]
    drawerPaper = styles.create [
      style.width (length.px drawerWidth)
      // Example of breakpoint media queries
      style.inner theme.breakpoints.downXs [
        style.backgroundColor.red
      ]
    ]
    content = styles.create [
      style.flexGrow 1
      style.padding (theme.spacing 3)
    ]
    toolbar = styles.create [
      yield! theme.mixins.toolbar
    ]
  |}
)

let Toolbar = React.functionComponent(fun (model, dispatch) ->
  let c = useToolbarStyles ()
  Mui.toolbar [
    Mui.typography [
      typography.variant.h6
      typography.color.inherit'
      typography.children [
          Mui.link [
              yield prop.text "Cookbook"
              yield link.color.inherit'
              yield! (Html.Props.routed Main)
          ]
      ]
      typography.classes.root c.appBarTitle
    ]
    Mui.link [
        yield prop.text "Login"
        yield! (Html.Props.routed Page.Login)
        yield button.color.inherit'
    ]
  ]
)

let private pageListItem (page:string) =
  Mui.listItem [
    prop.key page
    listItem.button true
    listItem.children [
      Mui.listItemText  page
    ]
  ]

let private drawer = React.functionComponent(fun (model,dispatch) ->
    let appStyles = useRootViewStyles ()
    Mui.drawer [
        drawer.variant.permanent
        drawer.classes.root appStyles.drawer
        drawer.classes.paper appStyles.drawerPaper
        drawer.children [
            Html.div [ prop.className appStyles.toolbar ]
            Mui.list [
                list.component' "nav"
                list.children (["test"] |> List.map pageListItem |> ofList)
            ]
        ]
    ]
)

let mainView = React.functionComponent(fun (model,dispatch) ->
    let appStyles = useRootViewStyles ()
    Html.main [
        prop.className appStyles.content
        prop.children [
            Html.div [ prop.className appStyles.toolbar ]
            Html.div "main"
        ]
    ]
)

let loginView = React.functionComponent(fun (model,(dispatch:Msg -> unit)) ->
    let appStyles = useRootViewStyles ()
    let loginProps :Login.State.LoginPageProps = { Login.State.handleNewToken = TokenChanged >> dispatch }
    Html.main [
        prop.className appStyles.content
        prop.children [
            Html.div [ prop.className appStyles.toolbar ]
            Pages.Login.View.render loginProps
        ]
    ]
)

let main = React.functionComponent(fun (model, (dispatch:Msg -> unit)) ->
    let appStyles = useRootViewStyles ()
    let lightTheme = Styles.createMuiTheme([
        theme.palette.type'.light
        theme.palette.primary Colors.indigo
        theme.palette.secondary Colors.pink
      ])

    Mui.themeProvider [
        themeProvider.theme lightTheme
        themeProvider.children [
            Html.div [
                prop.className appStyles.root
                prop.children [
                    yield Mui.cssBaseline []
                    yield Mui.appBar [
                        appBar.classes.root appStyles.appBar
                        appBar.position.fixed'
                        appBar.children [
                            Toolbar(model, dispatch)
                        ]
                    ]
                    match model.CurrentPage with
                    | Main ->
                        yield drawer (model, dispatch)
                        yield mainView (model, dispatch)
                    | Login ->
                        yield loginView (model, dispatch)

                ]
            ]
        ]
    ]
)

let render (model:State.Model) (dispatch: Msg -> unit) =

  let view = main (model, dispatch)
  React.router [
      router.pathMode
      router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
      router.children view
  ]